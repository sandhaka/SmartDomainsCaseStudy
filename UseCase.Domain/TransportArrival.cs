using System;

namespace UseCase.Domain
{
    public class TransportArrival : RecordData
    {
        public bool HadAccident { get; }
        public TimeSpan Delay { get; }

        public TransportArrival(
            DateTime time,
            int physicalStatusEvaluation, 
            string location, 
            string weather, 
            bool hadAccident, 
            TimeSpan delay) :
            base(physicalStatusEvaluation, location, weather, time)
        {
            HadAccident = hadAccident;
            Delay = delay;
        }
    }
}