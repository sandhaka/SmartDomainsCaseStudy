using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Search.Problem;
using Utils.Option;

namespace Ai.Infrastructure.Search.Walker.WalkerStrategies
{
    internal class DepthFirst<TState, TAction> : AbstractWalker<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly List<TState> _exploredSet;
        private readonly int _limit;

        public DepthFirst(ISearchProblem<TState, TAction> problem, int limit) : base(problem)
        {
            _exploredSet = new List<TState>();
            _limit = limit;
        }

        public override SearchExitCode Search()
        {
            var root = new Node<TState, TAction>(Problem.Initial, Problem.InitialNode);

            var exitCode = SearchExitCode.Failure;
            Node<TState, TAction> goalNode = null;

            Dls(1, root).IfNotNull(node =>
            {
                exitCode = SearchExitCode.Success;
                goalNode = node;
            });

            if (exitCode == SearchExitCode.Failure)
            {
                return exitCode;
            }

            Solution = goalNode.Path();

            return exitCode;
        }

        private Option<Node<TState, TAction>> Dls(int depth, Node<TState, TAction> node)
        {
            Iterations++;
            _exploredSet.Add(node.State);

            if (Problem.GoalTest(node.State))
            {
                return node;
            }

            if (_limit - depth == 0)
            {
                return new None<Node<TState, TAction>>();
            }

            var next = node.Expand(Problem).ToList();

            Node<TState, TAction> foundNode = null;

            foreach (var nextNode in next)
            {
                if (_exploredSet.Any(n => n.Equals(nextNode.State)))
                {
                    continue;
                }

                var found = false;

                Dls(depth + 1, nextNode).IfNotNull(f =>
                {
                    found = true;
                    foundNode = f;
                });

                if (found)
                {
                    return new Some<Node<TState, TAction>>(foundNode);
                }
            }

            return new None<Node<TState, TAction>>();
        }
    }
}