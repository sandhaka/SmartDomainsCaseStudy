using System.Collections.Generic;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class NoInference<T> : IInferenceStrategy<T>
        where T : CspValue
    {
        public bool Inference(Csp<T> csp, string varKey, T value, out IEnumerable<string> prunedDomains)
        {
            prunedDomains = new List<string>();
            return true;
        }
    }
}