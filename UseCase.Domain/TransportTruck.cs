using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventSourcing;
using Newtonsoft.Json;

namespace UseCase.Domain
{
    public class TransportTruck : EventSourcedAggregate
    {
        public Guid Id { get; }
        public string ModelCode { get; }
        public int Capacity { get; }
        public int FuelCapacity { get; }
        
        public TimeSpan CumulativeDelay { get; private set; }
        public string Location { get; private set; }
        public double PhysicalStatusEvaluationMeanLastWeek { get; private set; }
        public double PhysicalStatusEvaluationMeanLast30Days { get; private set; }
        public int TotalAccidents { get; private set; }

        private List<DomainEvent> LastChanges => Changes
            .OrderByDescending(c => c.Created)
            .ToList();

        public TransportTruck(string modelCode, int capacity, int fuelCapacity)
        {
            ModelCode = modelCode;
            Capacity = capacity;
            FuelCapacity = fuelCapacity;
        }
        
        public static TransportTruck Create(string modelCode, int capacity, int fuelCapacity)
        {
            return new TransportTruck(modelCode, capacity, capacity);
        }
        
        #region Bl

        public void Arrival(string to, DateTime? when = null, bool hadAccident = false, TimeSpan? delay = null)
        {
            when ??= DateTime.Now;
            delay ??= TimeSpan.Zero;

            var physicalStatusEvaluation = 0; // TODO
            string weather = ""; // TODO

            var arrivalEvent = new TransportArrival(
                when.Value, 
                physicalStatusEvaluation, 
                to, 
                weather, 
                hadAccident, 
                delay.Value);
            
            Causes(arrivalEvent);
        }

        public void Departure(string from, DateTime? when = null)
        {
            when ??= DateTime.Now;
            
            var physicalStatusEvaluation = 0; // TODO
            string weather = ""; // TODO

            var leaveEvent = new TransportDeparture(when.Value, physicalStatusEvaluation, from, weather);
            
            Causes(leaveEvent);
        }
        
        #endregion

        #region Event Sourcing impl
        
        protected override void When(DomainEvent @event)
        {
            // Temporary add the latest
            var latestChanges = LastChanges
                .Concat(new [] { @event })
                .Cast<RecordData>()
                .ToList();

            switch (@event)
            {
                // Handle arrival event stats
                case TransportArrival arrivalEvent:
                {
                    Location = arrivalEvent.Location;
                    CumulativeDelay += arrivalEvent.Delay;
                    PhysicalStatusEvaluationMeanLastWeek = latestChanges
                        .Take(7)
                        .Average(d => d.PhysicalStatusEvaluation);
                    PhysicalStatusEvaluationMeanLast30Days = latestChanges
                        .Take(30)
                        .Average(d => d.PhysicalStatusEvaluation);
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
                PhysicalStatusEvaluationMeanLastWeek,
                PhysicalStatusEvaluationMeanLastMonth = PhysicalStatusEvaluationMeanLast30Days,
                TotalAccidents
            });
        }

        protected override void LoadDataFromSnapshot(string data)
        {
            var aggregateData = JsonConvert.DeserializeObject<dynamic>(data);
            CumulativeDelay = aggregateData.CumulativeDelay;
            Location = aggregateData.Location;
            PhysicalStatusEvaluationMeanLastWeek = aggregateData.PhysicalStatusEvaluationMeanLastWeek;
            PhysicalStatusEvaluationMeanLast30Days = aggregateData.PhysicalStatusEvaluationMeanLastMonth;
            TotalAccidents = aggregateData.TotalAccidents;
        }
        
        #endregion
    }

    internal static class EnumerableExt
    {
        public static IEnumerable<TResult> FilterCast<TResult>(this IEnumerable source)
        {
            var s2 = source as IEnumerable<object>;
            return s2
                .Where(s => s.GetType() == typeof(TResult))
                .Cast<TResult>();
        }
    }
}