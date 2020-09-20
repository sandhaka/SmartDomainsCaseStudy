namespace Ai.Infrastructure.Csp.Csp
{
    public abstract class CspValue
    {
        public override bool Equals(object obj)
        {
            return TypeConcernedEquals(obj);
        }

        public override int GetHashCode()
        {
            return TypeConcernedGetHashCode();
        }

        public static bool operator ==(CspValue a, CspValue b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null))
            {
                return false;
            }

            if (ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(CspValue a, CspValue b)
        {
            return !(a == b);
        }

        public virtual void AssignmentCallback(string variableKey)
        {
            // Nothing to do
        }

        public virtual void RevokeCallback(string variableKey)
        {
            // Nothing to do
        }

        public abstract object Clone();

        protected abstract int TypeConcernedGetHashCode();

        protected abstract bool TypeConcernedEquals(object obj);
    }
}