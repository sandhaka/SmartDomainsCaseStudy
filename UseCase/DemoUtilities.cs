using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ai.Infrastructure.Search.Graph;
using Newtonsoft.Json;
using UseCase.DemoData;
using Xunit;

namespace UseCase
{
    public class DemoUtilities
    {
        private const int FleetSize = 10;
        private const int MaxHistoryNum = 100;
        private const int MinHistoryNum = 50;
        private readonly NorthItalyMap _map = new NorthItalyMap();
        
        private static Random _random = new Random();

        [Fact]
        public void GenerateDemoDataFile()
        {
            var data = new List<List<DemoHistory>>();

            for (var i = 0; i < FleetSize; i++)
            {
                var elemData = new List<DemoHistory>();

                var eventsNum = _random.Next(MinHistoryNum, MaxHistoryNum);
                var startNode = _map.Data[_random.Next(0, _map.Data.Count)];
                var arrivalNode = GetNextHopRandomly(startNode);
                
                var startTime = DateTime.UtcNow;

                for (var e = 0; e < eventsNum; e++)
                {
                    var startLocation = startNode.State;
                    var arrivalLocation = arrivalNode.State;
                    
                    var arrTime = GetJourneyDurationByPathCost(startTime, _map.PathCost(startLocation, arrivalLocation), out var delay);
                    
                    elemData.Add(new DemoHistory
                    {
                        DepartureLocation = startLocation.Name,
                        ArrivalLocation = arrivalLocation.Name,
                        Accident = false, // TODO: Very low probability
                        Delay = delay,
                        DepartureTime = startTime,
                        ArrivalTime = arrTime
                    });

                    startNode = arrivalNode;
                    arrivalNode = GetNextHopRandomly(startNode);

                    // Add rest time before next journey
                    startTime += arrTime - startTime + GetRealisticRestTime();
                }
                
                data.Add(elemData);
            }

            var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText($"../../demo_history{DateTime.Now.Ticks:x8}.json", jsonData);
        }

        private GraphNode<StringAim> GetNextHopRandomly(GraphNode<StringAim> startNode)
        {
            var n = startNode.Neighbors[_random.Next(0, startNode.Neighbors.Count)];
            return _map.Data.Single(d => d.State.Equals(n.State));
        }

        private DateTime GetJourneyDurationByPathCost(DateTime start, double cost, out TimeSpan delay)
        {
            delay = _random.Next(0, 10) <= 1 ? TimeSpan.FromMinutes(_random.Next(0, 120)) : TimeSpan.Zero;
            return start + TimeSpan.FromHours(cost / 100) + delay; // Assume 100Km/h as mean velocity
        }

        private TimeSpan GetRealisticRestTime()
        {
            var restHours = _random.Next(8, 24);
            return TimeSpan.FromHours(restHours);
        }
    }

    class DemoHistory
    {
        public string DepartureLocation { get; set; }
        public string ArrivalLocation { get; set; } 
        public DateTime DepartureTime { get; set; } 
        public DateTime ArrivalTime { get; set; }
        public TimeSpan Delay { get; set; }
        public bool Accident { get; set; }
    }
}