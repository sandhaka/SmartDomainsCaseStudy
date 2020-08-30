using System;

namespace EventSourcing
{
    public class StoredDomainEvent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
        public DateTime Created { get; set; }
        public string Data { get; set; }
    }
}