using System;

namespace Utils.Option
{
    public sealed class Some<T> : Option<T>
    {
        public T Content { get; }

        public Some(T value)
        {
            this.Content = value;
        }

        public static implicit operator T(Some<T> some) =>
            some.Content;

        public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
            map(this.Content);

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
            map(this.Content);

        public override T Reduce() =>
            this.Content;

        public override T Reduce(T whenNone) =>
            this.Content;

        public override T Reduce(Func<T> whenNone) =>
            this.Content;

        public override void IfNotNull(Action<T> act) => act(Content);
        public override bool IsNone => false;
    }
}