using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using EventSourcing.Exceptions;

namespace EventSourcing.Infrastructure
{
    /// <summary>
    /// Manage access to event sourced aggregate store
    /// </summary>
    /// <typeparam name="TEventSourced">Type of event sourced aggregate</typeparam>
    public class EventSourcedRepository<TEventSourced> : IEventSourcedRepository<TEventSourced>
        where TEventSourced : EventSourcedAggregate
    {
        private readonly IEventStore _eventStore;

        /// <summary>
        /// Stored events limit
        /// After 128 events stored a snapshot will be created automatically
        /// </summary>
        private const int Largest = 128;

        public EventSourcedRepository(IEventStore eventStore)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        }

        #region [ Api ]

        /// <summary>
        /// Find a stream given a id.
        /// Rebuild the aggregate to the current status applying all the events and return it
        /// </summary>
        /// <param name="id">Stream / Aggregate Id</param>
        /// <returns>Event Sourced Aggregate</returns>
        public TEventSourced FindById(Guid id)
        {
            Contract.Assume(id != null && !id.Equals(Guid.Empty));
            return InternalFindById(id);
        }

        /// <summary>
        /// Find a stream given a id and a desired date & time
        /// Rebuild the aggregate to the current status applying all the events and return it
        /// TODO: Deve essere possibile costruire l'aggregato usando un campo DateTime definito dal tipo di dato stesso oltre quello di default created. Per esempio nel caso di dati mutlitemporali.
        /// </summary>
        /// <param name="id">Stream / Aggregate Id</param>
        /// <param name="at">Less or equal date & time</param>
        /// <returns>Event Sourced Aggregate</returns>
        public TEventSourced FindById(Guid id, DateTime at)
        {
            Contract.Assume(id != null && !id.Equals(Guid.Empty));
            return InternalFindByIdAndTime(id, at);
        }

        /// <summary>
        /// Create a new stream
        /// </summary>
        /// <param name="aggregate">Event sourced aggregate</param>
        /// <returns>Task</returns>
        public void Add(TEventSourced aggregate)
        {
            Contract.Assume(aggregate?.Id != null && !aggregate.Id.Equals(Guid.Empty));

            var streamName = StreamNameFor(aggregate.Id);

            _eventStore.CreateNewStream(streamName);
        }

        /// <summary>
        /// Add uncommitted changes to an existent stream
        /// </summary>
        /// <param name="aggregate">Event sourced aggregate</param>
        /// <returns>Task</returns>
        public void Save(TEventSourced aggregate)
        {
            Contract.Assume(aggregate?.Id != null && !aggregate.Id.Equals(Guid.Empty));
            
            if (!aggregate.Changes.Any())
            {
                return;
            }

            SaveFunc(aggregate);
        }

        /// <summary>
        /// Allow optionally actions on aggregates and save it in a transactional scope
        /// </summary>
        /// <param name="action">Do your work</param>
        /// <param name="aggregates">Working entities</param>
        /// <returns></returns>
        public async Task DoMultiTransactionalWork(TEventSourced[] aggregates, Action<TEventSourced[]> action)
        {
            StartTransaction();

            try
            {
                action(aggregates);
                aggregates.ToList().ForEach(Save);
                await EndActiveTransactionAndCommitAsync();
            }
            catch (Exception)
            {
                await AbortActiveTransactionAsync();
                throw;
            }
        }

        /// <summary>
        /// Start a new transaction
        /// </summary>
        public void StartTransaction()
        {
            _eventStore.StarTransaction();
        }

        /// <summary>
        /// End transaction and commit changes
        /// </summary>
        /// <returns>Task</returns>
        public async Task EndActiveTransactionAndCommitAsync()
        {
            await _eventStore.EndActiveTransactionAndCommitAsync();
        }

        /// <summary>
        /// Abort current transaction
        /// </summary>
        /// <returns>Task</returns>
        public async Task AbortActiveTransactionAsync()
        {
            await _eventStore.AbortActiveTransactionAsync();
        }

        #endregion

        #region [ Private ]

        private void SaveFunc(TEventSourced aggregate)
        {
            // Avoid to add a largest number of events one shot. If you need to store more then 'Largest' events,
            // split the operation in two or more 'save' operation to keep stream size smaller, and allow
            // to create automatically one or more snapshot of the aggregate to drive the system
            // in performance optimization as result.
            if (aggregate.Changes.Count >= Largest)
            {
                throw new DomainEventsToAddLimitException($"If you need to store more then '{Largest}' events, " +
                                                          "split the operation in two or more 'save' operations to " +
                                                          "keep stream size smaller and allow to create automatically " +
                                                          "one or more snapshots");
            }

            var streamName = StreamNameFor(aggregate.Id);

            _eventStore.AppendEventsToStream(streamName, aggregate.Changes, aggregate.Version);

            // Get the stream size
            var streamSize = (int) _eventStore.GetEventsStreamSize(streamName);

            // Get the latest snapshot version
            var snapshot = _eventStore.GetLatestSnapshot(streamName);

            // Evaluate if snapshot creation is needed (after 'Largest' events stored)
            if ((streamSize - (snapshot?.Version ?? 0)) >= Largest)
            {
                MakeASnapshot(aggregate.Id);
            }
        }

        private TEventSourced CreateAggregate(Guid id)
        {
            var ctor = typeof(TEventSourced)
                .GetConstructors()
                .FirstOrDefault(c =>
                    c.IsPublic &&
                    c.GetParameters().Length == 1 &&
                    c.GetParameters().FirstOrDefault()?.ParameterType == typeof(Guid));

            return (TEventSourced) ctor?.Invoke(new object[] {id}) ??
                            throw new InvalidOperationException(
                                $"Type: {typeof(TEventSourced)} " +
                                $"must have a public constructor with Guid as unique parameter");
        }

        private TEventSourced RebuildAggregateFromEvents(TEventSourced aggregate, IEnumerable<DomainEvent> events)
        {
            // Rebuild the aggregate status applying all the events on the latest snapshot or
            // on a new aggregate generated from scratch
            foreach(var @event in events)
            {
                aggregate?.Apply(@event);
            }

            return aggregate;
        }

        private TEventSourced InternalFindByIdAndTime(Guid id, DateTime at)
        {
            var streamName = StreamNameFor(id);

            // Check latest snapshot before the requested time
            var snapshot = _eventStore.GetSnapshot(streamName, at);

            // Create a new aggregate instance
            var aggregate = CreateAggregate(id);

            // If there is a snapshot load from it before
            if (snapshot != null)
            {
                aggregate.LoadFromSnapshot(snapshot);
            }

            // Retrieves the version of the corresponding aggregate that is less than or equal to the specified date
            var toVersion = _eventStore.GetVersionAt(streamName, at);

            // No aggregate version found, return a null object
            if (snapshot == null && toVersion == null)
            {
                return null;
            }

            // Get events From aggregate version (zero or the version of the snapshot)
            var fromEventNumber = aggregate.Version;
            // To the desired version specified
            var toEventNumber = toVersion ?? 0;

            var storageStream = _eventStore.GetStream(streamName, fromEventNumber, toEventNumber);

            var stream = storageStream.ToList();

            // No aggregate version found, return a null object
            if (!stream.Any() && snapshot == null)
            {
                return null;
            }

            // Rebuild the aggregate from his events
            return RebuildAggregateFromEvents(aggregate, stream);
        }

        private TEventSourced InternalFindById(Guid id)
        {
            var streamName = StreamNameFor(id);

            // Check latest snapshot
            var snapshot = _eventStore.GetLatestSnapshot(streamName);

            // Create a new aggregate instance
            var aggregate = CreateAggregate(id);

            // If there is a snapshot load events from it before
            if (snapshot != null)
            {
                aggregate.LoadFromSnapshot(snapshot);
            }

            // Get events from aggregate version (zero or the version of the snapshot)
            var fromEventNumber = aggregate.Version;
            // Until the latest version
            var toEventNumber = fromEventNumber + Largest;

            var storageStream = _eventStore.GetStream(streamName, fromEventNumber, toEventNumber);

            var stream = storageStream.ToList();

            // No aggregate version found, return a null object
            if (!stream.Any() && snapshot == null)
            {
                return null;
            }

            // Rebuild the aggregate from his events
            return RebuildAggregateFromEvents(aggregate, stream);
        }

        private string StreamNameFor(Guid id)
        {
            return $"{typeof(TEventSourced).Name}-{id}";
        }

        private void MakeASnapshot(Guid id)
        {
            var aggregate = FindById(id);

            var snapshot = aggregate.Snapshot();

            _eventStore.AddSnapshot(StreamNameFor(aggregate.Id), snapshot);
        }

        #endregion
    }
}