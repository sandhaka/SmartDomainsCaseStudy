using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ai.Infrastructure.Csp.Csp;
using ConsoleTables;
using Csp.Resolvers.BackTrackingSearch.Parametric;
using TransportFleet.Domain;
using TransportFleet.UseCase.AiModels;
using TransportFleet.UseCase.DemoData.DataGenerationsUtils;
using TransportFleet.UseCase.Infrastructure;
using Xunit;

namespace TransportFleet.UseCase
{
    public class TestCase
    {
        private List<TransportTruck> _fleet;

        #region [ Setup ]

        private void PopulateWithEvents()
        {
            DemoLogger.InfLog($"Fleet with {_fleet.Count} units created");
            DemoLogger.InfLog("Adding historical data to fleet...");
            AddFleetDemoHistory();
            DemoLogger.InfLog(Environment.NewLine, false);
            DemoLogger.InfLog("Fleet historical data initialization done");
        }
        
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
        }

        #endregion
        
        /// <summary>
        /// Predict future events outcome with [Bayesian Networks].
        /// ---
        /// In this example smart domain capabilities is used to predict the accident probability based on
        /// statistical data collected for each truck.
        /// The predict method is exposed in the domain layer, bringing us to obtain a real-time information embedding
        /// these "smart" features directly in the domain objects.
        /// This is a good resonance example of Event Sourcing with 4.0 technologies.
        /// </summary>
        [Fact]
        public void ChooseTruckCandidateWithLowerAccidentProbability()
        {
            // Estimation on bad weather condition
            const bool goodWeather = false;
         
            DemoLogger.InfLog("Create fleet ...");
            _fleet = DemoFleetDataFactory.CreateRandomFleet(3);
            
            PopulateWithEvents();
            
            // Get results
            DemoLogger.InfLog("Estimate the probability to have an accident:");

            var rank = new Dictionary<Guid, double>();
            
            _fleet.ForEach(truck =>
            {
                // Trigger internal model updating
                truck.UpdateStats();
                
                var prediction = truck.PredictAccident(goodWeather);
                var prob = prediction.Probabilities.ToList();
                
                DemoLogger.InfLog($"Truck {truck.ModelCode}: {prob[0].Label}-> {prob[0].ProbabilityScore:P}, " +
                                  $"{prob[1].Label}-> {prob[1].ProbabilityScore:P}");
                
                rank.Add(truck.Id, prob[1].ProbabilityScore);
            });

            var candidateId = rank.OrderBy(c => c.Value).First().Key;
            var candidate = _fleet.Find(f => f.Id.Equals(candidateId));
            
            DemoLogger.InfLog($"Best truck: {candidate}");
        }
        
        /// <summary>
        /// Auto-assignment trips to fleet members by [optimization algorithms].
        /// ---
        ///  
        /// </summary>
        [Fact]
        public void OptimizeAndAssignTrips()
        {
            DemoLogger.InfLog("Create fleet ...");
            _fleet = DemoFleetDataFactory.CreateRandomFleet(6);
            
            // Create Csp data problem from current fleet status
            var cspData = FleetCsp.FleetToCspModel(DemoFleetDataFactory.CreateCspProblemVariables().ToList(), _fleet);
            
            // Instantiate Csp-Model
            var fleetCsp = CspFactory.Create(
                new Dictionary<string, IEnumerable<TruckCspValue>>(cspData.Select(v => new KeyValuePair<string, IEnumerable<TruckCspValue>>(v.Key, v.Value.Domains))),
                new Dictionary<string, IEnumerable<string>>(cspData.Select(v => new KeyValuePair<string, IEnumerable<string>>(v.Key, v.Value.Relations))),
                new Func<string, TruckCspValue, string, TruckCspValue, bool>[]
                {
                    // Constraint: Do not overlap on the same day
                    (variableA, transportTruckA, variableB, transportTruckB) =>
                    {
                        var dayA = variableA.Split('.').First();
                        var dayB = variableB.Split('.').First();
                        return dayA != dayB || transportTruckA != transportTruckB;
                    },    
                    // Avoid commitment on Monday and Tuesday
                    (variableA, transportTruckA, variableB, transportTruckB) =>
                    {
                        var dayA = variableA.Split('.').First();
                        var dayB = variableB.Split('.').First();
                        if (
                            string.Compare(dayA, "mon", StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(dayB, "tue", StringComparison.OrdinalIgnoreCase) == 0 ||
                            string.Compare(dayA, "tue", StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(dayB, "mon", StringComparison.OrdinalIgnoreCase) == 0
                        )
                        {
                            return transportTruckA != transportTruckB;
                        }

                        return true;
                    }
                });
            
            // Resolve the given problem
            var solved = fleetCsp.UseBackTrackingSearchResolver(
                    SelectUnassignedVariableStrategyTypes<TruckCspValue>.FirstUnassignedVariable,
                    DomainValuesOrderingStrategyTypes<TruckCspValue>.DomainCustomOrder)
                .Resolve(()=>
                {
                    PrintPlan(fleetCsp);
                });
            
            Assert.True(solved);
        }

        private void PrintPlan(Csp<TruckCspValue> model)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms);

            var table = new ConsoleTable("Week Day","Travel 1","Travel 2","Travel 3")
            {
                Options = {EnableCount = false, OutputTo = writer}
            };
            
            var travelsByDay = model.Status.GroupBy(k => DemoFleetDataFactory.DecodeDay(k.Key).ToUpper()).ToList();

            foreach (var day in travelsByDay)
            {
                var cells = day.Select(i => $"{i.Key}: {i.Value.ModelCode}").Cast<object>().ToList();
                cells.Insert(0, day.Key);
                
                table.AddRow(cells.ToArray());
            }

            table.Write();
            
            var trucks = model.Status.Values
                .Select(m => m.ModelCode)
                .Distinct()
                .Select(code => (Code: code, Assignement: model.Status.Values.First(v => v.ModelCode.Equals(code)).Assigned));

            foreach (var truck in trucks)
                writer.WriteLine($"Truck: {truck.Code}, Total travels: {truck.Assignement}");
            
            writer.Flush();
            
            ms.Seek(0, SeekOrigin.Begin);
            
            DemoLogger.InfLog(Environment.NewLine + Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}