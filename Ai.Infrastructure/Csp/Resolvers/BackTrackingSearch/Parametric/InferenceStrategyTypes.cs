using Ai.Infrastructure.Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    public static class InferenceStrategyTypes<T>
        where T : CspValue
    {
        public static readonly string NoInference = typeof(NoInference<T>).AssemblyQualifiedName;
        public static readonly string ForwardChecking = typeof(ForwardChecking<T>).AssemblyQualifiedName;
    }
}