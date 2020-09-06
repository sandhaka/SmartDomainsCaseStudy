using System.Collections.Generic;
using UseCase.Domain;
using UseCase.Infrastructure;
using UseCase.TestFactories;
using Xunit;

namespace UseCase
{
    /// <summary>
    /// 
    /// </summary>
    public class TestCase
    {
        private List<TransportTruck> _fleet;

        #region [ Setup ]
        
        /// <summary>
        /// 
        /// </summary>
        public TestCase()
        {
            _fleet = DemoFleetDataFactory.CreateRandomFleet(3);
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddFleetDemoHistory()
        {
            var c = 0;
            foreach (var transportTruck in _fleet)
            {
                var journeys = DemoFleetDataFactory.ReadNextJourneyHistory(c++);

                foreach (var journey in journeys)
                {
                    transportTruck.Departure(
                        journey.DepartureLocation,
                        PhysicalMeasurementFacade.Measure(transportTruck),
                        WeatherServiceFacade.Forecast(transportTruck),
                        journey.DepartureTime);
                    
                    transportTruck.Arrival(
                        journey.ArrivalLocation, 
                        PhysicalMeasurementFacade.Measure(transportTruck),
                        WeatherServiceFacade.Forecast(transportTruck),
                        journey.ArrivalTime, 
                        journey.Accident,
                        journey.Delay
                    );   
                }
            }
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