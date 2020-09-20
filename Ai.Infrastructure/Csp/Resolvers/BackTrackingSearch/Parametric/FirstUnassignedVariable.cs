using Ai.Infrastructure.Csp.Csp;
using Ai.Infrastructure.Csp.Csp.Model;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class FirstUnassignedVariable<T> : ISelectUnassignedVariableStrategy<T>
        where T : CspValue
    {
        public Variable<T> Next(Csp<T> csp)
        {
            return csp.Model.GetFirstVariable(v => !v.Assigned);
        }
    }
}