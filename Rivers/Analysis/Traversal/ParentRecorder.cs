using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    public class ParentRecorder
    {
        private readonly IDictionary<Node, Edge> _parents = new Dictionary<Node, Edge>();
        private readonly ITraversal _traversal;

        public ParentRecorder(ITraversal traversal)
        {
            _traversal = traversal ?? throw new ArgumentNullException(nameof(traversal));
            traversal.NodeDiscovered += (sender, args) =>
            {
                if (!_parents.ContainsKey(args.NewNode))
                    _parents[args.NewNode] = args.Origin;
            };
        }

        public Edge GetParentEdge(Node node)
        {
            return _parents[node];
        }

        public Node GetParent(Node node)
        {
            return _parents[node].GetOtherNode(node);
        }
    }
}