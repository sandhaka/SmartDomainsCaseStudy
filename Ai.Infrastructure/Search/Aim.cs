using System;
using System.Diagnostics;

namespace Ai.Infrastructure.Search
{
    [DebuggerDisplay("Name= {Name}")]
    public class Aim : IEquatable<Aim>, IComparable<Aim>
    {
        public string Name { get; }

        protected Aim()
        {
            Name = string.Empty;
        }

        protected Aim(string name)
        {
            Name = name;
        }

        public virtual bool Equals(Aim other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            return other.Name == Name;
        }

        public virtual int CompareTo(Aim other)
        {
            if (ReferenceEquals(other, null))
            {
                throw new NullReferenceException("CompareTo null object");
            }

            if (ReferenceEquals(other, this))
            {
                return 0;
            }

            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }
    }
}