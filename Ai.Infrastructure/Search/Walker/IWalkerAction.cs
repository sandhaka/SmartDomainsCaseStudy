namespace Ai.Infrastructure.Search.Walker
{
    /// <summary>
    /// Represent an "Action" in the graph case, an action is the next node
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public interface IWalkerAction<out TState>
        where TState : Aim
    {
        TState State { get; }
    }
}