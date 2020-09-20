using System.Collections.Generic;
using Ai.Infrastructure.Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public class DomainCustomOrder<T> : IDomainValuesOrderingStrategy<T>
        where T : CspValue
    {
        public IEnumerable<T> GetDomainValues(Csp<T> csp, string key)
        {
            var values = csp.Model.GetDomain(key).Values;
            values.Sort();
            return values;
        }
    }
}