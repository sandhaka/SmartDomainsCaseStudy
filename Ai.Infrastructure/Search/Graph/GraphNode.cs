using System.Collections.Generic;
using System.Diagnostics;
using Ai.Infrastructure.Search.Walker;

namespace Ai.Infrastructure.Search.Graph
{
    [DebuggerDisplay("State= {State.Name}, Cost= {Cost}")]
    public class GraphNode<TState> : IWalkerAction<TState>
        where TState : Aim
    {
        public TState State { get; }
        public double Cost { get; }
        public List<GraphNode<TState>> Neighbors { get; }

        public GraphNode(TState state, double cost)
        {
            State = state;
            Cost = cost;
            Neighbors = new List<GraphNode<TState>>();
        }
    }
}