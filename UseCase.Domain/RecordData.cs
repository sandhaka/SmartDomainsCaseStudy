using System;
using EventSourcing;

namespace UseCase.Domain
{
    public abstract class RecordData : DomainEvent
    {
        public int PhysicalStatusEvaluation { get; set; }
        public string Location { get; set; }
        public string Weather { get; set; }
        
        protected RecordData() : base(Guid.NewGuid(), DateTime.UtcNow)
        {
            
        }
    }
}