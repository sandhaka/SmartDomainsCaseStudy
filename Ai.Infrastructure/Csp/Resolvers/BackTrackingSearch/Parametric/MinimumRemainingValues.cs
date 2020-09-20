using System;
using System.Linq;
using Ai.Infrastructure.Csp.Csp;
using Ai.Infrastructure.Csp.Csp.Model;

namespace Csp.Resolvers.BackTrackingSearch.Parametric
{
    internal class MinimumRemainingValues<T> : ISelectUnassignedVariableStrategy<T>
        where T : CspValue
    {
        public Variable<T> Next(Csp<T> csp)
        {
            var vars = csp.Model.UnassignedVariables.Select(key =>
                {
                    var domain = csp.Model.GetDomain(key);
                    var legalValues = domain.Pruned.Any()
                        ? domain.Values.Count
                        : domain.Values.Count(c => csp.Model.Conflicts(key, c) == 0);

                    return new
                    {
                        Key = key,
                        LegalValuesCount = legalValues
                    };
                })
                .GroupBy(i => i.LegalValuesCount)
                .OrderBy(i => i.Key)
                .First()
                .ToList();

            return csp.Model.GetVariable(
                vars.Count > 1 ?
                    vars[new Random().Next(vars.Count)].Key :
                    vars.Single().Key
            );
        }

    }
}