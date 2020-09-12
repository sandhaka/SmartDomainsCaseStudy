namespace Ai.Infrastructure.Csp.Model
{
    internal class Variable<T>
        where T : CspValue
    {
        private T _value;

        internal string Key { get; }

        internal T Value
        {
            get => _value;
            set
            {
                if (value == null)
                {
                    _value?.RevokeCallback();
                    _value = null;
                }
                else
                {
                    _value = value;
                    _value.AssignmentCallback();
                }
            }
        }

        internal bool Assigned => Value != null;

        internal Variable(string key)
        {
            Key = key;
        }

        internal object ToAnonymous()
        {
            var val = Value?.Clone();
            return new
            {
                Key,
                val
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

            return (obj as Variable<T>)?.Key == Key;
        }

        public override int GetHashCode() => Key.GetHashCode();
    }
}