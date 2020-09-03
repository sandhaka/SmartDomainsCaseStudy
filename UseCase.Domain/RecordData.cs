using System;
using EventSourcing;

namespace UseCase.Domain
{
    public abstract class RecordData : DomainEvent
    {
        public int PhysicalStatusEvaluation { get; }
        public string Location { get; }
        public string Weather { get; }

        protected RecordData(int physicalStatusEvaluation, string location, string weather, DateTime created) : 
            base(null, created)
        {
            PhysicalStatusEvaluation = physicalStatusEvaluation;
            Location = location;
            Weather = weather;
        }
    }
}