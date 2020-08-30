using System;

namespace UseCase.Domain
{
    public class TransportArrival : RecordData
    {
        public bool HadAccident { get; set; }
        public TimeSpan Delay { get; set; }
    }
}