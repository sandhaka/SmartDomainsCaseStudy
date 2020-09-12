using System.Collections.Generic;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch
{
    internal interface IInferenceStrategy<T>
        where T : CspValue
    {
        bool Inference(Csp<T> csp, string varKey, T value, out IEnumerable<string> prunedDomains);
    }
}