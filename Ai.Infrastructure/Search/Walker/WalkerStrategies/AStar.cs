using System;
using System.Collections.Generic;
using System.Linq;
using Ai.Infrastructure.Search.Problem;
using Utils.Option;

namespace Ai.Infrastructure.Search.Walker.WalkerStrategies
{
    internal class AStar<TState, TAction> : AbstractWalker<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly Func<string, string, double> _heuristic;
        private readonly List<HeuristicNode<TState, TAction>> _frontier;

        public AStar(
            ISearchProblem<TState, TAction> problem,
            Func<string, string, double> heuristic) : base(problem)
        {
            _heuristic = heuristic;

            var root = new HeuristicNode<TState, TAction>(Problem.Initial, Problem.InitialNode);
            root.HeuristicPathCost = _heuristic(root.State.Name, Problem.Goal.Name);
            _frontier = new List<HeuristicNode<TState, TAction>> { root };
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

                var face = node.Expand(Problem).Select(HeuristicNode<TState, TAction>.From).ToList();

                face.ForEach(frontNode =>
                {
                    if (!Explored.Any(s => s.Equals(frontNode.State)) &&
                        !_frontier.Any(n => n.State.Equals(frontNode.State)))
                    {
                        AddToFrontier(frontNode);
                    }
                    else
                    {
                        _frontier.FirstOrNone(n =>
                                n.State.Equals(frontNode.State) &&
                                n.PathCost + _heuristic(n.State.Name, Problem.Goal.Name) >
                                frontNode.PathCost + _heuristic(frontNode.State.Name, Problem.Goal.Name))
                            .IfNotNull(n =>
                            {
                                _frontier.Remove(n);
                                AddToFrontier(frontNode);
                            });
                    }
                });
            }

            return SearchExitCode.Failure;
        }

        private void AddToFrontier(HeuristicNode<TState, TAction> node)
        {
            node.HeuristicPathCost = node.PathCost + _heuristic(node.State.Name, Problem.Goal.Name);
            _frontier.Add(node);
            _frontier.Sort();
        }
    }
}