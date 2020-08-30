using System;

namespace EventSourcing
{
    public class EventSourcedAggregateSnapshot
    {
        public EventSourcedAggregateSnapshot()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; }
        public DateTime Created { get; set; }
        public int Version { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }
}