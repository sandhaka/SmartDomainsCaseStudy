using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Search.Graph;
using Ai.Infrastructure.Search.Walker;
using Utils.Option;

namespace Ai.Infrastructure.Search.Problem
{
    public abstract class SearchProblem<TState, TAction> : ISearchProblem<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly Graph<TState> _graph;

        public TState Goal { get; }

        public TState Initial { get; }

        protected SearchProblem(Graph<TState> graph, TState initial, TState goal)
        {
            _graph = graph;
            Initial = initial;
            Goal = goal;
        }

        public virtual IEnumerable<TAction> Actions(TState state) =>
            GetNode(state).Map(n => n.Neighbors).Reduce().Cast<TAction>();

        public GraphNode<TState> InitialNode => GetNode(Initial).Reduce();

        public virtual bool GoalTest(TState state)
        {
            return Goal.Equals(state);
        }

        public virtual double PathCost(double cost, TState fromState, TAction action, TState toState)
        {
            return cost + GetNode(fromState)
                .Map(n => n.Neighbors)
                .Reduce(new List<GraphNode<TState>>())
                .FirstOrNone(n => n.State.Name.Equals(toState.Name))
                .Map(s => s.Cost)
                .Reduce(0);
        }

        public virtual TState Result(TState state, TAction action)
        {
            return action.State;
        }

        protected virtual Option<GraphNode<TState>> GetNode(TState state)
        {
            return _graph.Data.FirstOrNone(g => g.State.Equals(state));
        }
    }
}