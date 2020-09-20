using System;

namespace TransportFleet.Domain
{
    public class TransportDeparture : RecordData
    {
        public TransportDeparture(DateTime time, double fatigue, string location, string weather, bool weatherIsGood) : 
            base(fatigue, location, weather, time, weatherIsGood)
        {
        }
    }
}