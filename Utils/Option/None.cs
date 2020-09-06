using System;

namespace Utils.Option
{
    public sealed class None<T> : Option<T>
    {
        public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
            None.Value;

        public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
            None.Value;

        public override T Reduce() =>
            default(T);

        public override T Reduce(T whenNone) =>
            whenNone;

        public override T Reduce(Func<T> whenNone) =>
            whenNone();

        public override void IfNotNull(Action<T> act) { }
        public override bool IsNone => true;
    }

    public sealed class None
    {
        public static None Value { get; } = new None();

        private None() { }
    }
}