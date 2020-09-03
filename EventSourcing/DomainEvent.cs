using System;
using Newtonsoft.Json;

namespace EventSourcing
{
    /// <summary>
    /// Domain event base class
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// Event id
        /// </summary>
        public Guid Id { get; }
        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Created { get; }

        protected DomainEvent(Guid? id = null, DateTime? created = null)
        {
            Created = created ?? DateTime.UtcNow;
            Id = id ?? Guid.NewGuid();
        }

        /// <summary>
        /// Convert a domain event in a storable version
        /// </summary>
        /// <returns>Storable domain event: <see cref="StoredDomainEvent"/></returns>
        public StoredDomainEvent ToStorableDto()
        {
            return new StoredDomainEvent
            {
                Type = GetType().AssemblyQualifiedName,
                Created = Created,
                Data = JsonConvert.SerializeObject(this)
            };
        }
    }
}