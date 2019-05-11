using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    /// <summary>
    /// Provides a mechanism for recording a post traversal order.
    /// </summary>
    public class PostOrderRecorder
    {
        private readonly Stack<Node> _stack = new Stack<Node>();
        private readonly List<Node> _order = new List<Node>();

        public PostOrderRecorder(ITraversal traversal)
        {
            traversal.NodeDiscovered += TraversalOnNodeDiscovered;
            traversal.TraversalCompleted += TraversalOnTraversalCompleted;
        }

        /// <summary>
        /// Gets the final post-order of nodes that was recorded.
        /// </summary>
        /// <returns></returns>
        public IList<Node> GetOrder()
        {
            return _order.AsReadOnly();
        }

        private void TraversalOnNodeDiscovered(object sender, NodeDiscoveryEventArgs e)
        {
            if (e.Origin == null)
            {
                AddRemainingNodes();
            }
            else
            {
                var originNode = e.Origin.GetOtherNode(e.NewNode);
                while (_stack.Peek() != originNode)
                    _order.Add(_stack.Pop());
            }

            _stack.Push(e.NewNode);
        }

        private void TraversalOnTraversalCompleted(object sender, EventArgs e)
        {
            AddRemainingNodes();
        }

        private void AddRemainingNodes()
        {
            while (_stack.Count > 0)
                _order.Add(_stack.Pop());
        }
    }
}