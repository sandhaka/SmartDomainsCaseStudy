using System;
using Ai.Infrastructure.Search.Problem;
using Ai.Infrastructure.Search.Walker.WalkerStrategies;

namespace Ai.Infrastructure.Search.Walker
{
    /// <summary>
    /// Create walker search strategies
    /// </summary>
    public static class WalkerFactory
    {
        /// <summary>
        /// Expands the shallowest nodes first.
        /// It is complete, optimal for unit step costs, but has exponential space complexity.
        /// </summary>
        /// <param name="problem">Search problem</param>
        /// <typeparam name="TState">State type</typeparam>
        /// <typeparam name="TAction">Action type</typeparam>
        /// <returns>Walker instance</returns>
        public static IWalker<TState> CreateBreadthFirst<TState, TAction>(ISearchProblem<TState, TAction> problem)
            where TState : Aim
            where TAction : IWalkerAction<TState>
        {
            return new BreadthFirst<TState, TAction>(problem);
        }

        /// <summary>
        /// Expands the node with lowest path cost is optimal for general step costs.
        /// </summary>
        /// <param name="problem">Search problem</param>
        /// <typeparam name="TState">State type</typeparam>
        /// <typeparam name="TAction">Action type</typeparam>
        /// <returns>Walker instance</returns>
        public static IWalker<TState> CreateUniformCost<TState, TAction>(ISearchProblem<TState, TAction> problem)
            where TState : Aim
            where TAction : IWalkerAction<TState>
        {
            return new UniformCost<TState, TAction>(problem);
        }

        /// <summary>
        /// Expands the deepest unexpanded node first. It is neither complete nor optimal,
        /// but has linear space complexity. Depth-limited search adds a depth bound.
        /// </summary>
        /// <param name="problem">Search problem</param>
        /// <param name="depthLimit">A depth limit</param>
        /// <typeparam name="TState">State type</typeparam>
        /// <typeparam name="TAction">Action type</typeparam>
        /// <returns>Walker instance</returns>
        public static IWalker<TState> CreateDepthFirst<TState, TAction>(ISearchProblem<TState, TAction> problem, int depthLimit = 90)
            where TState : Aim
            where TAction : IWalkerAction<TState>
        {
            return new DepthFirst<TState, TAction>(problem, depthLimit);
        }

        /// <summary>
        /// robust, optimal search algorithm that use limited amounts of memory; give enough time,
        /// it can solve problems that A∗ cannot solve because it runs out of memory.
        /// </summary>
        /// <param name="problem">Search problem</param>
        /// <param name="heuristic">Heuristic as Func</param>
        /// <typeparam name="TState">State type</typeparam>
        /// <typeparam name="TAction">Action type</typeparam>
        /// <returns>Walker instance</returns>
        public static IWalker<TState> CreateRecursiveBestFirst<TState, TAction>(ISearchProblem<TState, TAction> problem, Func<string ,string, double> heuristic)
            where TState : Aim
            where TAction : IWalkerAction<TState>
        {
            return new RecursiveBestFirst<TState, TAction>(problem, heuristic);
        }

        /// <summary>
        /// Expands nodes with minimal f(n) = g(n) + h(n). A∗ is complete and optimal, provided that h(n) is admissible
        /// (for TREE-SEARCH) or consistent (for GRAPH-SEARCH). The space complexity of A∗ is still prohibitive.
        /// </summary>
        /// <param name="problem">Search problem</param>
        /// <param name="heuristic">Heuristic as Func</param>
        /// <typeparam name="TState">State type</typeparam>
        /// <typeparam name="TAction">Action type</typeparam>
        /// <returns>Walker instance</returns>
        public static IWalker<TState> CreateAStar<TState, TAction>(ISearchProblem<TState, TAction> problem, Func<string ,string, double> heuristic)
            where TState : Aim
            where TAction : IWalkerAction<TState>
        {
            return new AStar<TState, TAction>(problem, heuristic);
        }
    }
}