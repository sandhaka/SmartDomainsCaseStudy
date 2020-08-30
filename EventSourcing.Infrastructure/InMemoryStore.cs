using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventSourcing.Infrastructure
{
    public class InMemoryStore : IEventStore
    {
        private readonly ConcurrentDictionary<int, List<StoredDomainEvent>> _streams;
        private readonly ConcurrentDictionary<int, List<EventSourcedAggregateSnapshot>> _snapshots;

        private List<StoredDomainEvent> GetStream(string streamName) => _streams[streamName.ToStableHash()];

        private List<EventSourcedAggregateSnapshot> GetSnapshotsStream(string streamName)
        {
            var key = streamName.ToStableHash();
            if (!_snapshots.TryGetValue(key, out var snapshots))
            {
                snapshots = new List<EventSourcedAggregateSnapshot>();
                _snapshots.TryAdd(key, snapshots);
            }

            return snapshots;
        }

        public InMemoryStore()
        {
            _streams = new ConcurrentDictionary<int, List<StoredDomainEvent>>();
            _snapshots = new ConcurrentDictionary<int, List<EventSourcedAggregateSnapshot>>();
        }
        
        #region Api
        
        public void CreateNewStream(string streamName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            var key = streamName.ToStableHash();
            _streams.TryAdd(key, new List<StoredDomainEvent>());
        }

        public void AppendEventsToStream(string streamName, IEnumerable<DomainEvent> domainEvents, int expectedVersion)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            Contract.Requires<ArgumentException>(domainEvents != null || domainEvents.Any(), "No events to add");
            Contract.Requires<ArgumentException>(expectedVersion >= 0, "Invalid expected version, must be >=0");
            var stream = GetStream(streamName);
            var storableEvents = domainEvents.Select(e => e.ToStorableDto()).ToList();
            foreach (var domainEvent in storableEvents)
            {
                domainEvent.Id = Guid.NewGuid().ToString();
                domainEvent.Version = expectedVersion++;
            }
            stream.AddRange(storableEvents);
            Contract.Ensures(stream.Any(), "No events added");
        }

        public long GetEventsStreamSize(string streamName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            return GetStream(streamName).Count;
        }

        public IEnumerable<DomainEvent> GetStream(string streamName, int fromVersion, int toVersion)
        {
            Contract.Requires<ArgumentException>(toVersion >= fromVersion && toVersion >= 0 && fromVersion >= 0);
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            var stream = GetStream(streamName)
                .Skip(fromVersion)
                .Take(toVersion - fromVersion)
                .ToList();

            return stream.Select(evt => (DomainEvent) JsonConvert.DeserializeObject(evt.Data, Type.GetType(evt.Type)))
                .ToList();
        }

        public int? GetVersionAt(string streamName, DateTime at)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            var stream = GetStream(streamName);
            return stream
                .Where(f => f.Created <= at)
                .OrderByDescending(f => f.Version)
                .FirstOrDefault()?.Version;
        }

        public bool Exists(string streamName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            return _streams.ContainsKey(streamName.ToStableHash());
        }

        public void AddSnapshot(string streamName, EventSourcedAggregateSnapshot snapshot)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            var snapshots = GetSnapshotsStream(streamName);
            snapshots.Add(snapshot);
        }

        public EventSourcedAggregateSnapshot GetLatestSnapshot(string streamName)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            return GetSnapshotsStream(streamName)
                .OrderByDescending(s => s.Version)
                .FirstOrDefault();
        }

        public EventSourcedAggregateSnapshot GetSnapshot(string streamName, DateTime at)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(streamName), "Null stream name");
            return GetSnapshotsStream(streamName)
                .Where(s => s.Created <= at)
                .OrderByDescending(s => s.Version)
                .FirstOrDefault();
        }

        public void StarTransaction()
        {
            throw new NotImplementedException("not supported");
        }

        public Task EndActiveTransactionAndCommitAsync()
        {
            throw new NotImplementedException("not supported");
        }

        public Task AbortActiveTransactionAsync()
        {
            throw new NotImplementedException("not supported");
        }
        
        #endregion
    }
    
    static class StringExt
    {
        internal static int ToStableHash(this string str) => jenkins_one_at_a_time_hash(str, str.Length);

        private static int jenkins_one_at_a_time_hash(string s, int length) {
            var i = 0;
            var hash = 0;
            while (i != length) {
                hash += s[i++];
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;
            return hash;
        }
    }
}