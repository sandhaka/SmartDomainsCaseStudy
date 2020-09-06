namespace Ai.Infrastructure.Search.Walker
{
    /// <summary>
    /// Walker node represent a problem status in the search three
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public interface IWalkerNode<out TState>
        where TState : Aim
    {
        TState State { get; }
        double PathCost { get; }
    }
}