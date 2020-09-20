using Ai.Infrastructure.Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public static class SelectUnassignedVariableStrategyTypes<T>
        where T : CspValue
    {
        public static readonly string FirstUnassignedVariable = typeof(FirstUnassignedVariable<T>).AssemblyQualifiedName;
        public static readonly string MinimumRemainingValues = typeof(MinimumRemainingValues<T>).AssemblyQualifiedName;
    }
}