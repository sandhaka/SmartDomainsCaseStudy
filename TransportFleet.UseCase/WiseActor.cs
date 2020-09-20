using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.Bayes;
using TransportFleet.Domain;

namespace TransportFleet.UseCase
{
    public class WiseActor : IWiseActor
    {
        private readonly IDictionary<Guid, NaiveBayes> _bayesianModel = 
            new Dictionary<Guid, NaiveBayes>();
        
        private readonly NaiveBayesLearning _learner;

        public WiseActor()
        {
            _learner = new NaiveBayesLearning();
        }
        
        public void Update(TransportTruck transportTruck)
        {
            var translatedData = Parse(transportTruck.Changes.Cast<RecordData>());
            
            var itemModel = _learner.Learn(translatedData.x, translatedData.y);
            
            if (!_bayesianModel.ContainsKey(transportTruck.Id))
            {
                _bayesianModel.Add(transportTruck.Id, itemModel);
                return;
            }

            _bayesianModel[transportTruck.Id] = itemModel;
        }

        public (int Answer, IEnumerable<(string Label, double ProbabilityScore)> Probabilities) FutureAccidentIncidence(
            Guid transportTruckId,
            bool goodWeatherCondition, 
            TimeSpan delay,
            double fatigue)
        {
            var input = ToPattern(goodWeatherCondition, (int) delay.TotalMinutes, fatigue);
            var answer = _bayesianModel[transportTruckId].Decide(input);
            var prob = _bayesianModel[transportTruckId].Probabilities(input);

            return (answer, new[]
            {
                (Label: "No", ProbabilityScore: prob[0]),
                (Label: "Yes", ProbabilityScore: prob[1])
            });
        }

        private static (int[][] x, int[] y) Parse(IEnumerable<RecordData> history)
        {
            var historyList = history.ToList();
            var input = new List<int[]>();
            var output = new List<int>();
            
            historyList.ForEach(current =>
            {
                if (current is TransportArrival tra)
                {
                    input.Add(ToPattern(tra.GoodWeather, (int)tra.Delay.TotalMinutes, tra.Fatigue));
                    output.Add(tra.HadAccident ? 1 : 0);
                }
            });

            return (input.ToArray(), output.ToArray());
        }

        private static int[] ToPattern(bool goodWeather, int delayInMinutes, double fatigue) => 
            new []
            {
                goodWeather ? 1 : 0,
                delayInMinutes > 5 ? 1 : 0,
                fatigue > 0.25 ? 1 : 0
            };
    }
}