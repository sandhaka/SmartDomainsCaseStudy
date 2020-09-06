using System.Collections.Generic;
using System.Linq;
using Utils.Option;

namespace Ai.Infrastructure.Search.Graph
{
    public class UndirectedGraph<TState> : Graph<TState>
        where TState : Aim
    {
        protected UndirectedGraph(IEnumerable<GraphNode<TState>> data) : base(data)
        {
            MakeUndirected();
        }

        private void MakeUndirected()
        {
            var data = Data.ToList();

            data.ForEach(node =>
            {
                node.Neighbors.ForEach(childNode => Connect(node, childNode));
            });
        }

        private void Connect(GraphNode<TState> aNode, GraphNode<TState> bNode)
        {
            var node = Data.FirstOrNone(n => bNode.State.Equals(n.State));

            if (node.IsNone)
            {
                Data.Add(new GraphNode<TState>(bNode.State, 0)
                {
                    Neighbors =
                    {
                        new GraphNode<TState>(aNode.State, bNode.Cost)
                    }
                });
            }
            else
            {
                var child = node.Reduce().Neighbors.FirstOrNone(nn => nn.State.Equals(aNode.State));
                if (child.IsNone)
                {
                    node.Reduce().Neighbors.Add(new GraphNode<TState>(aNode.State, bNode.Cost));
                }
            }
        }
    }
}