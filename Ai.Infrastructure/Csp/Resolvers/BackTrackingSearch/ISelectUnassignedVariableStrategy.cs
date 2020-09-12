using Ai.Infrastructure.Csp.Model;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch
{
    internal interface ISelectUnassignedVariableStrategy<T>
        where T : CspValue
    {
        Variable<T> Next(Csp<T> csp);
    }
}