using System;

namespace UseCase.Domain
{
    public class TransportDeparture : RecordData
    {
        public TransportDeparture(DateTime time, int physicalStatusEvaluation, string location, string weather) : 
            base(physicalStatusEvaluation, location, weather, time)
        {
        }
    }
}