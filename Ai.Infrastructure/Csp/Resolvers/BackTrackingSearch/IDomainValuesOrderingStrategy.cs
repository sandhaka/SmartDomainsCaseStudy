using System.Collections.Generic;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch
{
    public interface IDomainValuesOrderingStrategy<T>
        where T : CspValue
    {
        IEnumerable<T> GetDomainValues(Csp<T> csp, string key);
    }
}