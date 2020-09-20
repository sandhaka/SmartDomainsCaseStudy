using Ai.Infrastructure.Csp.Csp;
using Ai.Infrastructure.Csp.Csp.Model;

namespace Csp.Resolvers.BackTrackingSearch
{
    internal interface ISelectUnassignedVariableStrategy<T>
        where T : CspValue
    {
        Variable<T> Next(Csp<T> csp);
    }
}