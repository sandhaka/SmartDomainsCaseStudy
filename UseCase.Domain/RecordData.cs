using System;
using EventSourcing;

namespace UseCase.Domain
{
    public abstract class RecordData : DomainEvent
    {
        public double Fatigue { get; }
        public string Location { get; }
        public string Weather { get; }
        public bool GoodWeather { get; }

        protected RecordData(double fatigue, string location, string weather, DateTime created, bool goodWeather) : 
            base(null, created)
        {
            Fatigue = fatigue;
            Location = location;
            Weather = weather;
            GoodWeather = goodWeather;
        }
    }
}