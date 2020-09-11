using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EventSourcing;
using Newtonsoft.Json;
using Utils;

namespace UseCase.Domain
{
    [DebuggerDisplay("Code= {ModelCode} " +
                     "Current Location= {Location} " +
                     "AverageLastWeekFatigue= {AverageFatigueLastWeek} " +
                     "AverageLastWeekDelay= {AverageDelayLastWeek} " +
                     "TotalAccidents= {TotalAccidents} " +
                     "Events= {Changes.Count}")]
    public class TransportTruck : EventSourcedAggregate
    {
        private readonly IWiseActor _aiActor;
        public string ModelCode { get; }
        public int Capacity { get; }
        public int FuelCapacity { get; }
        public TimeSpan CumulativeDelay { get; private set; }
        public string Location { get; private set; }
        public double AverageFatigueLastWeek { get; private set; }
        public double AverageFatigueLast30Days { get; private set; }
        public TimeSpan AverageDelayLastWeek { get; private set; }
        public int TotalAccidents { get; private set; }
        public Action<string, bool> DomainLog { get; set; }

        private List<DomainEvent> LastChanges => Changes
            .OrderByDescending(c => c.Created)
            .ToList();

        public TransportTruck(Guid? id, string modelCode, int capacity, int fuelCapacity, IWiseActor aiActor) :
            base(id ?? Guid.NewGuid())
        {
            ModelCode = modelCode;
            Capacity = capacity;
            FuelCapacity = fuelCapacity;
            _aiActor = aiActor;
        }

        public override string ToString() => $"Code= {ModelCode} " +
                                             $"Current Location= {Location} " +
                                             $"AverageLastWeekFatigue= {AverageFatigueLastWeek} " +
                                             $"AverageLastWeekDelay= {AverageDelayLastWeek} " +
                                             $"TotalAccidents= {TotalAccidents} " +
                                             $"Events= {Changes.Count}";
        
        #region Bl

        public (int Answer, IEnumerable<(string Label, double ProbabilityScore)> Probabilities) PredictAccident(bool goodWeatherCondition = true) =>
            _aiActor.FutureAccidentIncidence(Id, goodWeatherCondition, AverageDelayLastWeek, AverageFatigueLastWeek);

        public void UpdateStats()
        {
            _aiActor.Update(this);
        }
        
        public void Arrival(
            string to, 
            double fatigue, 
            string weather, 
            bool weatherIsGood,
            DateTime? when = null,
            bool hadAccident = false,
            TimeSpan? delay = null)
        {
            when ??= DateTime.Now;
            delay ??= TimeSpan.Zero;

            var arrivalEvent = new TransportArrival(
                when.Value, 
                fatigue, 
                to, 
                weather, 
                weatherIsGood,
                hadAccident, 
                delay.Value);
            
            Causes(arrivalEvent);
        }

        public void Departure(string from, double fatigue, string weather, bool weatherIsGood,  DateTime? when = null)
        {
            when ??= DateTime.Now;

            var leaveEvent = new TransportDeparture(when.Value, fatigue, from, weather, weatherIsGood);
            
            Causes(leaveEvent);
        }
        
        #endregion

        #region Event Sourcing impl
        
        protected override void When(DomainEvent @event)
        {
            DomainLog?.Invoke(".", false);
            
            // Temporary add the latest
            var latestChanges = LastChanges
                .Cast<RecordData>()
                .ToList();

            switch (@event)
            {
                // Handle arrival event stats
                case TransportArrival arrivalEvent:
                {
                    Location = arrivalEvent.Location;
                    CumulativeDelay += arrivalEvent.Delay;
                    AverageDelayLastWeek = TimeSpan.FromMinutes(
                        latestChanges
                            .FilterCast<TransportArrival>()
                            .Take(7)
                            .Average(evt => evt.Delay.TotalMinutes));
                    AverageFatigueLastWeek = latestChanges
                        .FilterCast<TransportArrival>()
                        .Take(7)
                        .Average(d => d.Fatigue);
                    AverageFatigueLast30Days = latestChanges
                        .FilterCast<TransportArrival>()
                        .Take(30)
                        .Average(d => d.Fatigue);
                    TotalAccidents = latestChanges
                        .FilterCast<TransportArrival>()
                        .Count(evt => evt.HadAccident);
                    
                    break;
                }
                case TransportDeparture departureEvent:
                {
                    Location = "-";
                    break;
                }
            }
        }
        
        protected override string SerializeAggregateData()
        {
            return JsonConvert.SerializeObject(new
            {
                CumulativeDelay,
                Location,
                FatigueMeanLastWeek = AverageFatigueLastWeek,
                FatigueMeanLast30Days = AverageFatigueLast30Days,
                TotalAccidents
            });
        }

        protected override void LoadDataFromSnapshot(string data)
        {
            var aggregateData = JsonConvert.DeserializeObject<dynamic>(data);
            CumulativeDelay = aggregateData.CumulativeDelay;
            Location = aggregateData.Location;
            AverageFatigueLastWeek = aggregateData.FatigueMeanLastWeek;
            AverageFatigueLast30Days = aggregateData.FatigueMeanLast30Days;
            TotalAccidents = aggregateData.TotalAccidents;
        }
        
        #endregion
    }
}