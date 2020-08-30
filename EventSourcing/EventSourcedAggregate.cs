using System;
using System.Collections.Generic;

namespace EventSourcing
{
    public abstract class EventSourcedAggregate
    {
        public Guid Id { get; protected set; }

        public int Version { get; protected set; }

        public int InitialVersion { get; protected set; }

        /// <summary>
        /// Collection of uncommitted events
        /// </summary>
        public List<DomainEvent> Changes { get; protected set; }

        /// <summary>
        /// Parameter-less constructor is needed by the events repository to create new instances
        /// </summary>
        protected EventSourcedAggregate()
        {
            Changes = new List<DomainEvent>();
        }

        /// <summary>
        /// Ctor to build aggregate from a stored stream
        /// </summary>
        /// <param name="id">Aggregate Id</param>
        protected EventSourcedAggregate(Guid id)
        {
            Changes = new List<DomainEvent>();
            Id = id;
        }

        /// <summary>
        /// Create a snapshot from the current state
        /// </summary>
        /// <returns></returns>
        public EventSourcedAggregateSnapshot Snapshot()
        {
            return new EventSourcedAggregateSnapshot
            {
                Version = Version,
                Type = GetType().AssemblyQualifiedName,
                Created = DateTime.Now,
                Data = SerializeAggregateData()
            };
        }

        /// <summary>
        /// Routine to serialize aggregate specific data
        /// </summary>
        /// <returns>Serialized payload</returns>
        protected abstract string SerializeAggregateData();

        /// <summary>
        /// Load from a saved snapshot
        /// </summary>
        /// <param name="snapshot">Snapshot</param>
        public void LoadFromSnapshot(EventSourcedAggregateSnapshot snapshot)
        {
            Version = snapshot.Version;
            InitialVersion = snapshot.Version;
            LoadDataFromSnapshot(snapshot.Data);
        }

        /// <summary>
        /// Routine to load aggregate specific data
        /// </summary>
        /// <param name="data">Json data</param>
        protected abstract void LoadDataFromSnapshot(string data);

        /// <summary>
        /// Takes a DomainEvent that aggregates must handle by applying business rules and updating state.
        /// </summary>
        /// <param name="event">Event</param>
        public virtual void Apply(DomainEvent @event)
        {
            When(@event);
            Version++;
        }

        /// <summary>
        /// Is intended to read almost declaratively, expressing what business rules should apply and how the state
        /// should change when each type of event occurs.
        /// </summary>
        /// <param name="event">Event</param>
        protected abstract void When(DomainEvent @event);

        /// <summary>
        /// Changes the collection of uncommitted events and then feeding the event into Apply method.
        /// </summary>
        /// <param name="event">Event</param>
        protected virtual void Causes(DomainEvent @event)
        {
            Changes.Add(@event);
            Apply(@event);
        }
    }
}