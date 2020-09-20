using System.Linq;
using Ai.Infrastructure.Csp.Csp;

namespace Csp.Resolvers.BackTrackingSearch
{
    internal class BackTrackingSearch<T> : IResolver<T>
        where T : CspValue
    {
        private readonly ISelectUnassignedVariableStrategy<T> _selUnVarStrategy;
        private readonly IDomainValuesOrderingStrategy<T> _domValOrdStrategy;
        private readonly IInferenceStrategy<T> _infStrategy;

        public BackTrackingSearch(
            ISelectUnassignedVariableStrategy<T> selUnVarStrategy,
            IDomainValuesOrderingStrategy<T> domValOrdStrategy,
            IInferenceStrategy<T> infStrategy)
        {
            _selUnVarStrategy = selUnVarStrategy;
            _domValOrdStrategy = domValOrdStrategy;
            _infStrategy = infStrategy;
        }

        public bool Resolve(Csp<T> csp)
        {
            if (csp.Model.IsAllAssigned)
            {
                return true;
            }

            var variable = _selUnVarStrategy.Next(csp);
            foreach (var domainValue in _domValOrdStrategy.GetDomainValues(csp, variable.Key).ToList())
            {
                if (csp.Conflicts(variable.Key, domainValue) == 0)
                {
                    csp.AddAssignment(variable.Key, domainValue);
                    csp.Model.Suppose(variable.Key, domainValue);
                    if (_infStrategy.Inference(csp, variable.Key, domainValue, out var pruned))
                    {
                        if (Resolve(csp))
                        {
                            return true;
                        }
                    }

                    foreach (var prunedDomain in pruned)
                    {
                        csp.Model.RestorePruned(prunedDomain);
                    }

                    csp.Model.RestoreGuess(variable.Key);
                }
            }
            csp.RemoveAssignment(variable.Key);
            return false;
        }
    }
}