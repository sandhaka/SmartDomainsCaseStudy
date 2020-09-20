using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Ai.Infrastructure.Csp.Csp.Model
{
    internal class Domain<T>
        where T : CspValue
    {
        internal string Key { get; }
        internal List<T> Values { get; }
        internal List<T> Pruned { get; }
        internal List<T> RemovedByGuess { get; }

        internal bool IsEmpty => !Values.Any();

        internal Domain(string key, IEnumerable<T> values)
        {
            Contract.Assert(values.Any());

            Key = key;
            Values = values.ToList();
            Pruned = new List<T>();
            RemovedByGuess = new List<T>();
        }

        internal T Random()
        {
            return Values[new Random().Next(Values.Count)];
        }

        internal void Prune(T value)
        {
            var v = Values.Find(val => val.Equals(value));
            Pruned.Add(v);
            Values.Remove(v);
        }

        internal void Shrink(T value)
        {
            Values.RemoveAll(v => v != value);
        }

        internal void Suppose(T value)
        {
            RemovedByGuess.AddRange(Values.Where(v => v != value));
            Values.RemoveAll(v => v != value);
        }

        internal void RestoreGuess()
        {
            Values.AddRange(RemovedByGuess);
            RemovedByGuess.Clear();
        }

        internal void RestorePruned()
        {
            Values.AddRange(Pruned);
            Pruned.Clear();
        }

        internal object ToAnonymous()
        {
            return new
            {
                Key,
                Values = Values.ToList()
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

            return (obj as Domain<T>)?.Key == Key;
        }

        public override int GetHashCode() => Key.GetHashCode();

    }
}