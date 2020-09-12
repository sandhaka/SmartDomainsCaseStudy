using System.Collections.Generic;
using System.Linq;

namespace Ai.Infrastructure.Csp.Resolvers.BackTrackingSearch.Parametric
{
    public class ForwardChecking<T> : IInferenceStrategy<T>
        where T : CspValue
    {
        public bool Inference(Csp<T> csp, string varKey, T value, out IEnumerable<string> prunedDomains)
        {
            var prunedSet = new HashSet<string>();
            foreach (var unassignedNeighbor in csp.Model.VariableRelations(varKey).Values.Where(v => !v.Assigned))
            {
                foreach (var val in csp.Model.GetDomain(unassignedNeighbor.Key).Values.ToList())
                {
                    if (csp.Model.GetConstraints().Any(c => !c.Rule.Invoke(varKey, value, unassignedNeighbor.Key, val)))
                    {
                        csp.Model.Prune(unassignedNeighbor.Key, val);
                        prunedSet.Add(unassignedNeighbor.Key);
                    }
                }

                if (!csp.Model.GetDomain(unassignedNeighbor.Key).Values.Any())
                {
                    prunedDomains = prunedSet.ToList();
                    return false;
                }
            }

            prunedDomains = prunedSet.ToList();
            return true;
        }
    }
}