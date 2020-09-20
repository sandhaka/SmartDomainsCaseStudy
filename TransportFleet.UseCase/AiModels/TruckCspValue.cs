using System;
using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Csp.Csp;
using TransportFleet.Domain;
using TransportFleet.UseCase.DemoData.DataGenerationsUtils;
using Utils;

namespace TransportFleet.UseCase.AiModels
{
    /// <summary>
    /// Modelling transport truck in Constrain satisfaction problem context
    /// </summary>
    public class TruckCspValue : CspValue, IComparable<TruckCspValue>
    {
        public string ModelCode { get; }
        public int Assigned { get; private set; }

        public TruckCspValue(string modelCode, int assigned = 0)
        {
            ModelCode = modelCode;
            Assigned = assigned;
        }
        
        public int CompareTo(TruckCspValue other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Assigned.CompareTo(other.Assigned);
        }

        public override object Clone()
        {
            return new TruckCspValue(ModelCode, Assigned);
        }

        protected override int TypeConcernedGetHashCode() => ModelCode.GetHashCode();

        protected override bool TypeConcernedEquals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b2 = (TruckCspValue)obj;
            return ModelCode == b2.ModelCode;
        }

        public override void AssignmentCallback(string variableKey)
        {
            Assigned++;
        }

        public override void RevokeCallback(string variableKey)
        {
            Assigned--;
        }
    }
    
    /// <summary>
    /// Convert fleet in a csp model problem
    /// </summary>
    public static class FleetCsp
    {
        public static Dictionary<string, (List<TruckCspValue> Domains, List<string> Relations)> FleetToCspModel(
            List<string> variables,
            List<TransportTruck> fleet)
        {
            // Domain values are the truck itself as a csp model value
            var domains = fleet.Select(t => new TruckCspValue(t.ModelCode)).ToList();
            // Relations lists
            var relations = new Dictionary<string, List<string>>();
            
            // Create a simple correlation about the travel day
            foreach (var day in DemoFleetDataFactory.Days)
            {
                // Select travels on the same day
                var sameDayVariables = variables
                    .Where(v => DemoFleetDataFactory.DecodeDay(v).Equals(day))
                    .ToList();
                
                // These travels are related due of their day
                sameDayVariables.ForEach(v => 
                    relations.Add(v, sameDayVariables.Where(vv => vv != v).ToList())
                );
            }

            // Pack all
            var data = new Dictionary<string, (List<TruckCspValue> domains, List<string> relations)>(
                variables.Select(v => new KeyValuePair<string, (List<TruckCspValue> domains, List<string> relations)>(
                    v, (domains, relations.Where(r => r.Key == v).SelectMany(r => r.Value).ToList())
                ))
            );

            return data;
        }
    }
}