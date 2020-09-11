using System;
using System.Collections.Generic;
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
                        journey.DepartureTime);
                    
                    transportTruck.Arrival(
                        journey.ArrivalLocation, 
                        journey.FatigueScore,
                        journey.WeatherCode,
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
        
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void RunDemo()
        {
            /* Demo setup */
            AddFleetDemoHistory();
            
        }
    }
}