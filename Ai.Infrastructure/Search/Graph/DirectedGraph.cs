using System.Collections.Generic;

namespace Ai.Infrastructure.Search.Graph
{
    internal class DirectedGraph<TState> : Graph<TState> where TState : Aim
    {
        public DirectedGraph(IEnumerable<GraphNode<TState>> data) : base(data)
        {

        }
    }
}