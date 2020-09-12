using Ai.Infrastructure.Csp.Model;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch.Parametric
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