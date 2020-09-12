using System.Collections.Generic;
using System.Linq;

namespace Ai.Infrastructure.Csp.Model
{
    internal class Relations<T>
        where T : CspValue
    {
        internal string Key { get; }
        internal List<Variable<T>> Values { get; }

        internal Relations(string key, IEnumerable<Variable<T>> values)
        {
            Key = key;
            Values = values.ToList();
        }

        internal object ToAnonymous()
        {
            return new
            {
                Key,
                Values = Values.Select(v => v.ToAnonymous())
            };
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

            return (obj as Relations<T>)?.Key == Key;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}