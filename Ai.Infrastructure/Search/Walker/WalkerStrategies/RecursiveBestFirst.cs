using System;
using System.Linq;
using Ai.Infrastructure.Search.Problem;
using Utils.Option;

namespace Ai.Infrastructure.Search.Walker.WalkerStrategies
{
    internal class RecursiveBestFirst<TState, TAction> : AbstractWalker<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        private readonly Func<string, string, double> _heuristic;

        public RecursiveBestFirst(
            ISearchProblem<TState, TAction> problem, Func<string, string, double> heuristic) : base(problem)
        {
            _heuristic = heuristic;
        }

        public override SearchExitCode Search()
        {
            Iterations = 0;

            var root = new HeuristicNode<TState, TAction>(Problem.Initial, Problem.InitialNode);
            root.HeuristicPathCost = _heuristic(root.State.Name, Problem.Goal.Name);
            var res = RbFs(root, double.PositiveInfinity);

            if (res.Node.IsNone)
            {
                return SearchExitCode.Failure;
            }

            Solution = res.Node.Reduce().Path();
            return SearchExitCode.Success;
        }

        private PathResult RbFs(HeuristicNode<TState, TAction> node, double limit)
        {
            Iterations++;

            if (Problem.GoalTest(node.State))
            {
                return new PathResult(node, node.HeuristicPathCost);
            }

            var successors = node.Expand(Problem).Select(HeuristicNode<TState, TAction>.From).ToList();

            if (!successors.Any())
            {
                return new PathResult(new None<HeuristicNode<TState, TAction>>(), double.PositiveInfinity);
            }

            foreach (var childNode in successors)
            {
                var childHCost = childNode.PathCost + _heuristic(childNode.State.Name, Problem.Goal.Name);
                childNode.HeuristicPathCost = Math.Max(childHCost, node.HeuristicPathCost);
            }

            while (true)
            {
                successors.Sort();

                var best = successors.FirstOrNone();

                if (best.IsNone)
                {
                    return new PathResult(new None<HeuristicNode<TState, TAction>>(), double.PositiveInfinity);
                }

                if (best.Reduce().HeuristicPathCost > limit)
                {
                    return new PathResult(new None<HeuristicNode<TState, TAction>>(), best.Reduce().HeuristicPathCost);
                }

                var alternativeCost = successors.Count() > 1 ?
                    successors.ElementAt(1).HeuristicPathCost :
                    double.PositiveInfinity;

                var res = RbFs(best.Reduce(), Math.Min(limit, alternativeCost));
                best.Reduce().HeuristicPathCost = res.Cost;

                if (!res.Node.IsNone)
                {
                    return new PathResult(res.Node, best.Reduce().HeuristicPathCost);
                }
            }
        }

        private class PathResult
        {
            public Option<HeuristicNode<TState, TAction>> Node { get; }
            public double Cost { get; }

            public PathResult(Option<HeuristicNode<TState, TAction>> node, double cost)
            {
                Node = node;
                Cost = cost;
            }
        }
    }
}