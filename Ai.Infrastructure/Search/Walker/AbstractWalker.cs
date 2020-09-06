using System.Collections.Generic;
using Ai.Infrastructure.Search.Problem;

namespace Ai.Infrastructure.Search.Walker
{
    internal abstract class AbstractWalker<TState, TAction> : IWalker<TState>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        protected readonly ISearchProblem<TState, TAction> Problem;
        protected readonly Stack<TState> Explored;

        public IEnumerable<IWalkerNode<TState>> Solution { get; protected set; }
        public int Iterations { get; protected set; }

        protected AbstractWalker(ISearchProblem<TState, TAction> problem)
        {
            Problem = problem;
            Explored = new Stack<TState>();
        }

        public abstract SearchExitCode Search();
    }
}