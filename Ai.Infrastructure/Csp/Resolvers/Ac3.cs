using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Csp.Csp;
using Ai.Infrastructure.Csp.Csp.Model;

// Note: readability is preferred
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Csp.Resolvers
{
    internal class Ac3<T> : IArcConsistency<T>
        where T : CspValue
    {
        private Queue<(string Key, Variable<T> Value)> _queue;

        public bool Propagate(Csp<T> csp)
        {
            _queue = new Queue<(string Key, Variable<T> Value)>(
                csp.Model.GetRelations.SelectMany(r => r.Values.Select(v => (r.Key, v)))
            );

            while (_queue.Any())
            {
                var pair = _queue.Dequeue();

                var x = csp.Model.GetVariable(pair.Key);

                var y = pair.Value;

                if (Revise(csp, x, y))
                {
                    if (csp.Model.GetDomain(x.Key).IsEmpty)
                    {
                        return false;
                    }

                    var relatedVariables = csp.Model.VariableRelations(x.Key).Values;

                    foreach (var variable in relatedVariables)
                    {
                        if (!_queue.ToList().Exists(i => i.Key.Equals(variable.Key) && i.Value.Equals(x)))
                        {
                            _queue.Enqueue((variable.Key, x));
                        }
                    }
                }
            }

            return true;
        }

        private bool Revise(Csp<T> csp, Variable<T> i, Variable<T> j)
        {
            var revised = false;

            foreach (var domainIVal in csp.Model.GetDomain(i.Key).Values.ToList())
            {
                if (csp.Model.GetDomain(j.Key)
                    .Values.All(domainJVal =>
                        csp.Model.GetConstraints().Any(c =>
                            !c.Rule.Invoke(i.Key, domainIVal, j.Key, domainJVal)
                        )
                    )
                )
                {
                    csp.Model.Prune(i.Key, domainIVal);
                    revised = true;
                }
            }

            return revised;
        }
    }
}