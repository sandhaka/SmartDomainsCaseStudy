using System;

namespace Ai.Infrastructure.Csp.Model
{
    internal class Constraint<T>
        where T : CspValue
    {
        internal Func<string, T, string, T, bool> Rule { get; }

        public Constraint(Func<string, T, string, T, bool> rule)
        {
            Rule = rule;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return (obj as Constraint<T>)?.Rule == Rule;
        }

        public override int GetHashCode() => Rule.GetHashCode();
    }
}