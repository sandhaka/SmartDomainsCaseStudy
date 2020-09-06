using System.Collections.Generic;
using System.Linq;

namespace Ai.Infrastructure.Search.Graph
{
    public abstract class Graph<TState> where TState : Aim
    {
        public List<GraphNode<TState>> Data { get; }

        protected Graph(IEnumerable<GraphNode<TState>> data)
        {
            Data = data.ToList();
        }
    }
}