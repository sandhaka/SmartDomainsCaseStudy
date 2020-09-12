using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Ai.Infrastructure.Csp
{
    public class CspFactory
    {
        public static Csp<T> Create<T>(
            IDictionary<string, IEnumerable<T>> domains,
            IDictionary<string, IEnumerable<string>> relations,
            IEnumerable<Func<string, T, string, T, bool>> constraints
        ) where T : CspValue
        {
            Contract.Assert(domains.Any());
            Contract.Assert(relations.Any());
            Contract.Assert(constraints.Any());

            return new Csp<T>(domains, relations, constraints);
        }
    }
}