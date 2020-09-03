using System;
using System.Collections.Generic;
using UseCase.Domain;
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
            foreach (var transportTruck in _fleet)
            {
                
                
                transportTruck.Departure("MILANO");
                transportTruck.Arrival("ROMA", 
                    DateTime.UtcNow + TimeSpan.FromHours(7), 
                    false,
                    TimeSpan.FromMinutes(45));
            }
        }

        [Fact]
        public void RunDemo()
        {
            AddFleetDemoHistory();
            
        }
    }
}