using System;
using System.Collections.Generic;

namespace UseCase.Domain
{
    /// <summary>
    /// Domain interface for AI algorithms
    /// </summary>
    public interface IWiseActor
    {
        public void Update(TransportTruck transportTruck);
        
        public (int Answer, IEnumerable<(string Label, double ProbabilityScore)> Probabilities) FutureAccidentIncidence(
            Guid transportTruckId,
            bool goodWeatherCondition,
            TimeSpan delay,
            double fatigue
            );
    }
}