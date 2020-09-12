using System.Collections.Generic;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class UnorderedDomainValues<T> : IDomainValuesOrderingStrategy<T>
        where T : CspValue
    {
        public IEnumerable<T> GetDomainValues(Csp<T> csp, string key)
        {
            return csp.Model.GetDomain(key).Values;
        }
    }
}