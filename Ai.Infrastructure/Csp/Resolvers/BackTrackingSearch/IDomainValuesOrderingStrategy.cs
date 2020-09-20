using System.Collections.Generic;
using Ai.Infrastructure.Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch
{
    public interface IDomainValuesOrderingStrategy<T>
        where T : CspValue
    {
        IEnumerable<T> GetDomainValues(Csp<T> csp, string key);
    }
}