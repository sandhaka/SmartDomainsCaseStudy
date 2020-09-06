using System;
using System.Collections.Generic;
using Ai.Infrastructure.Search.Problem;

namespace Ai.Infrastructure.Search.Walker
{
    internal class Node<TState, TAction> : IComparable<Node<TState, TAction>>, IWalkerNode<TState>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        public Node<TState, TAction> Parent { get; }
        public TState State { get; }
        public IWalkerAction<TState> Action { get; }
        public double PathCost { get; }

        public Node(TState state, IWalkerAction<TState> action, Node<TState, TAction> parent = null, double pathCost = 0)
        {
            Parent = parent;
            State = state;
            Action = action;
            PathCost = pathCost;
        }

        public IEnumerable<Node<TState, TAction>> Expand(ISearchProblem<TState, TAction> problem)
        {
            var actions = problem.Actions(State);

            foreach (var action in actions)
            {
                yield return ChildNode(problem, action);
            }
        }

        public IEnumerable<Node<TState, TAction>> Path()
        {
            var pathBack = new List<Node<TState, TAction>>();
            var node = this;
            while (node != null)
            {
                pathBack.Add(node);
                node = node.Parent;
            }
            pathBack.Reverse();
            return pathBack;
        }

        public int CompareTo(Node<TState, TAction> other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return CompareValues(other);
        }

        private Node<TState, TAction> ChildNode(ISearchProblem<TState, TAction> problem, TAction action)
        {
            var nextState = problem.Result(State, action);
            return new Node<TState, TAction>(
                nextState,
                action,
                this,
                problem.PathCost(
                    PathCost, State, action, nextState
                )
            );
        }

        protected virtual int CompareValues(Node<TState, TAction> other)
        {
            return PathCost.CompareTo(other.PathCost);
        }
    }
}