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
        public string ModelCode { get; }
        public int Capacity { get; }
        public int FuelCapacity { get; }
        
        public TimeSpan CumulativeDelay { get; private set; }
        public string Location { get; private set; }
        public double FatigueMeanLastWeek { get; private set; }
        public double FatigueMeanLast30Days { get; private set; }
        public int TotalAccidents { get; private set; }
        public Action<string, bool> DomainLog { get; set; }

        private List<DomainEvent> LastChanges => Changes
            .OrderByDescending(c => c.Created)
            .ToList();

        public TransportTruck(Guid? id, string modelCode, int capacity, int fuelCapacity) :
            base(id ?? Guid.NewGuid())
        {
            ModelCode = modelCode;
            Capacity = capacity;
            FuelCapacity = fuelCapacity;
        }
        
        #region Bl

        public void Arrival(
            string to, 
            double fatigue, 
            string weather, 
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
                hadAccident, 
                delay.Value);
            
            Causes(arrivalEvent);
        }

        public void Departure(string from, double fatigue, string weather,  DateTime? when = null)
        {
            when ??= DateTime.Now;

            var leaveEvent = new TransportDeparture(when.Value, fatigue, from, weather);
            
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
                    FatigueMeanLastWeek = latestChanges
                        .Take(7)
                        .Average(d => d.Fatigue);
                    FatigueMeanLast30Days = latestChanges
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
                FatigueMeanLastWeek,
                FatigueMeanLast30Days,
                TotalAccidents
            });
        }

        protected override void LoadDataFromSnapshot(string data)
        {
            var aggregateData = JsonConvert.DeserializeObject<dynamic>(data);
            CumulativeDelay = aggregateData.CumulativeDelay;
            Location = aggregateData.Location;
            FatigueMeanLastWeek = aggregateData.FatigueMeanLastWeek;
            FatigueMeanLast30Days = aggregateData.FatigueMeanLast30Days;
            TotalAccidents = aggregateData.TotalAccidents;
        }
        
        #endregion
    }

    internal static class EnumerableExt
    {
        public static IEnumerable<TResult> FilterCast<TResult>(this IEnumerable source)
        {
            var s2 = source as IEnumerable<object>;
            return s2!.Where(s => s.GetType() == typeof(TResult))
                .Cast<TResult>();
        }
    }
}