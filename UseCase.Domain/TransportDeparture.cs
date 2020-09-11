using System;

namespace UseCase.Domain
{
    public class TransportDeparture : RecordData
    {
        public TransportDeparture(DateTime time, double fatigue, string location, string weather) : 
            base(fatigue, location, weather, time)
        {
        }
    }
}