using System;
using System.Collections.Generic;
using System.Linq;
using UseCase.Domain;
using UseCase.Infrastructure;
using UseCase.TestFactories;
using Xunit;

namespace UseCase
{
    public class TestCase
    {
        private readonly List<TransportTruck> _fleet;

        #region [ Setup ]
        
        /// <summary>
        /// 
        /// </summary>
        public TestCase()
        {
            DemoLogger.InfLog("Create fleet ...");
            _fleet = DemoFleetDataFactory.CreateRandomFleet(3);
            DemoLogger.InfLog($"Fleet with {_fleet.Count} units created");
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddFleetDemoHistory()
        {
            DemoLogger.InfLog("Adding historical data to fleet...");
            
            var c = 0;
            foreach (var transportTruck in _fleet)
            {
                var journeys = DemoFleetDataFactory.ReadNextJourneyHistory(c++);

                foreach (var journey in journeys)
                {
                    transportTruck.Departure(
                        journey.DepartureLocation,
                        0,
                        journey.WeatherCode,
                        WeatherServiceFacade.IsGoodWeather(journey.WeatherCode),
                        journey.DepartureTime);
                    
                    transportTruck.Arrival(
                        journey.ArrivalLocation, 
                        journey.FatigueScore,
                        journey.WeatherCode,
                        WeatherServiceFacade.IsGoodWeather(journey.WeatherCode),
                        journey.ArrivalTime, 
                        journey.Accident,
                        journey.Delay
                    );   
                }
            }
            DemoLogger.InfLog(Environment.NewLine, false);
            DemoLogger.InfLog("Fleet historical data initialization done");
        }

        #endregion
        
        [Fact]
        public void RunDemo()
        {
            // Demo setup 
            AddFleetDemoHistory();

            // Estimation on these variables:
            const bool goodWeather = false;
            
            // Get results
            DemoLogger.InfLog("Estimate the probability to have an accident:");

            var rank = new Dictionary<Guid, double>();
            
            _fleet.ForEach(truck =>
            {
                // Trigger internal model updating
                truck.UpdateStats();
                
                var prediction = truck.PredictAccident(goodWeather);
                var prob = prediction.Probabilities.ToList();
                
                DemoLogger.InfLog($"Truck {truck.ModelCode}: {prob[0].Label}-> {prob[0].ProbabilityScore:P}, {prob[1].Label}-> {prob[1].ProbabilityScore:P}");
                
                rank.Add(truck.Id, prob[1].ProbabilityScore);
            });

            var candidate = _fleet.Find(f => f.Id.Equals(rank.OrderBy(c => c.Value).First().Key));
            
            DemoLogger.InfLog($"Best candidate truck for the next journey: {candidate}");
        }
    }
}