using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Search.Problem;
using Utils;
using Utils.Option;

namespace Ai.Infrastructure.Search.Walker.WalkerStrategies
{
    internal class UniformCost<TState, TAction> : AbstractWalker<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly List<Node<TState, TAction>> _frontier;

        public UniformCost(ISearchProblem<TState, TAction> problem) : base(problem)
        {
            var root = new Node<TState, TAction>(Problem.Initial, Problem.InitialNode);
            _frontier = new List<Node<TState, TAction>> { root };
        }

        public override SearchExitCode Search()
        {
            while (_frontier.Any())
            {
                Iterations++;
                var node = _frontier.First();
                _frontier.Remove(node);

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
                        _frontier.Add(frontNode);
                        _frontier.Sort();
                    }
                    else
                    {
                        _frontier.FirstOrNone(n => n.State.Equals(frontNode.State) && n.PathCost > frontNode.PathCost)
                            .IfNotNull(n =>
                            {
                                _frontier.Remove(n);
                                _frontier.Add(frontNode);
                                _frontier.Sort();
                            });

                    }
                });
            }

            return SearchExitCode.Failure;
        }
    }
}