using System.Collections.Generic;

namespace Ai.Infrastructure.Search.Walker
{
    /// <summary>
    /// Bind a problem and find a solution
    /// </summary>
    /// <typeparam name="TState">State type</typeparam>
    public interface IWalker<out TState>
        where TState : Aim
    {
        /// <summary>
        /// A collection of action performed to reach the solution (null if a solution is not been reached)
        /// </summary>
        IEnumerable<IWalkerNode<TState>> Solution { get; }
        /// <summary>
        /// Number of total iterations
        /// </summary>
        int Iterations { get; }
        /// <summary>
        /// Try to find a solution building a search three given a problem
        /// </summary>
        /// <returns>Exit code</returns>
        SearchExitCode Search();
    }
}