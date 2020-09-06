using System.Collections.Generic;
using Ai.Infrastructure.Search.Graph;
using Ai.Infrastructure.Search.Walker;

namespace Ai.Infrastructure.Search.Problem
{
    /// <summary>
    /// Search problem abstraction
    /// </summary>
    /// <typeparam name="TState">A type of problem internal state</typeparam>
    /// <typeparam name="TAction">A type of problem result action for each state. In common scenario can be
    /// another node of the graph</typeparam>
    public interface ISearchProblem<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        /// <summary>
        /// Initial state, prior knowledge
        /// </summary>
        TState Initial { get; }

        /// <summary>
        /// Relative root node of the search graph
        /// </summary>
        GraphNode<TState> InitialNode { get; }

        /// <summary>
        /// /// Goal, prior knowledge
        /// </summary>
        TState Goal { get; }

        /// <summary>
        /// Check if the state is the goal one
        /// </summary>
        /// <param name="state">State</param>
        /// <returns>True or False</returns>
        bool GoalTest(TState state);

        /// <summary>
        /// Return the available actions for this state
        /// </summary>
        /// <param name="state">State</param>
        /// <returns>Possible actions</returns>
        IEnumerable<TAction> Actions(TState state);

        /// <summary>
        /// Calculate the path cost from the start state to target state
        /// </summary>
        /// <param name="cost">Cumulative cost</param>
        /// <param name="fromState">Start state</param>
        /// <param name="action">Action to perform in transition</param>
        /// <param name="toState">Target state</param>
        /// <returns>Cost</returns>
        double PathCost(double cost, TState fromState, TAction action, TState toState);

        /// <summary>
        /// Return the state result by apply an action to a state
        /// </summary>
        /// <param name="state">State</param>
        /// <param name="action">Action</param>
        /// <returns>Next State</returns>
        TState Result(TState state, TAction action);
    }
}