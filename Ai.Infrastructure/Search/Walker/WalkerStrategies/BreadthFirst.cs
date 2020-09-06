using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Search.Problem;
using Utils;

namespace Ai.Infrastructure.Search.Walker.WalkerStrategies
{
    internal class BreadthFirst<TState, TAction> : AbstractWalker<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly Queue<Node<TState, TAction>> _frontier;

        public BreadthFirst(ISearchProblem<TState, TAction> problem) : base(problem)
        {
            var root = new Node<TState, TAction>(Problem.Initial, Problem.InitialNode);
            _frontier = new Queue<Node<TState, TAction>>();
            _frontier.Enqueue(root);
        }

        public override SearchExitCode Search()
        {
            while (_frontier.Any())
            {
                Iterations++;
                var node = _frontier.Dequeue();
                Explored.Push(node.State);

                if (Problem.GoalTest(node.State))
                {
                    Solution = node.Path();
                    return SearchExitCode.Success;
                }

                var face = node.Expand(Problem);

                face.ForEach(frontNode =>
                {
                    if (!Explored.Any(s => s.Equals(frontNode.State)) &&
                        !_frontier.Any(n => n.State.Equals(frontNode.State)))
                    {
                        _frontier.Enqueue(frontNode);
                    }
                });
            }

            return SearchExitCode.Failure;
        }
    }
}