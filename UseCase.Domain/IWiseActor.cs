using System;
using System.Collections.Generic;

namespace UseCase.Domain
{
    public interface IWiseActor
    {
        internal void Update(TransportTruck transportTruck);
        
        public (int Answer, IEnumerable<(string Label, double ProbabilityScore)> Probabilities) FutureAccidentIncidence(
            Guid transportTruckId,
            bool goodWeatherCondition,
            TimeSpan delay,
            double fatigue
            );
    }
}