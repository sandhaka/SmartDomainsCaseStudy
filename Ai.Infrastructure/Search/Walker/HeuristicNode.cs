namespace Ai.Infrastructure.Search.Walker
{
    internal class HeuristicNode<TState, TAction> : Node<TState, TAction>
        where TState : Aim
        where TAction : IWalkerAction<TState>
    {
        internal double HeuristicPathCost { get; set; }

        internal HeuristicNode(
            TState state,
            IWalkerAction<TState> action,
            Node<TState, TAction> parent = null,
            double pathCost = 0) : base(state, action, parent, pathCost)
        {
        }

        internal static HeuristicNode<TState, TAction> From(Node<TState, TAction> node)
        {
            return new HeuristicNode<TState, TAction>(node.State, node.Action, node.Parent, node.PathCost);
        }

        protected override int CompareValues(Node<TState, TAction> other)
        {
            var rbFsNode = other as HeuristicNode<TState, TAction>;

            if (HeuristicPathCost > rbFsNode!.HeuristicPathCost)
            {
                return 1;
            }

            if (HeuristicPathCost < rbFsNode!.HeuristicPathCost)
            {
                return -1;
            }

            return 0;
        }
    }
}