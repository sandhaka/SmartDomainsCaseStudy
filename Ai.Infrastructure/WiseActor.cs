using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.Bayes;
using UseCase.Domain;

namespace Ai.Infrastructure
{
    public class WiseActor : IWiseActor
    {
        private readonly IDictionary<Guid, Accord.MachineLearning.Bayes.NaiveBayes> _bayesianModel = 
            new Dictionary<Guid, Accord.MachineLearning.Bayes.NaiveBayes>();
        
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
            var input = new[]
            {
                goodWeatherCondition ? 1 : 0,
                (int) delay.TotalMinutes > 5 ? 1 : 0,
                fatigue > 0.25 ? 1 : 0
            };
            
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
                    input.Add(ToPattern(tra));
                    output.Add(tra.HadAccident ? 1 : 0);
                }
            });

            return (input.ToArray(), output.ToArray());
        }

        private static int[] ToPattern(TransportArrival tra) => 
            new []
            {
                tra.GoodWeather ? 1 : 0,
                (int) tra.Delay.TotalMinutes > 5 ? 1 : 0,
                tra.Fatigue > 0.25 ? 1 : 0
            };
    }
}