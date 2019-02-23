using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis.Traversal
{
    /// <summary>
    /// Provides a way to record the order in which nodes are discovered during a traversal of a graph.
    /// </summary>
    public class TraversalOrderRecorder
    {
        private readonly IDictionary<Node, int> _indices = new Dictionary<Node, int>();
        private readonly List<Node> _order = new List<Node>();
        private readonly ITraversal _traversal;

        public TraversalOrderRecorder(ITraversal traversal)
        {
            _traversal = traversal ?? throw new ArgumentNullException(nameof(traversal));
            _traversal.NodeDiscovered += TraversalOnNodeDiscovered;
        }
        
        /// <summary>
        /// Gets a collection of all the nodes that were discovered during the traversal.
        /// </summary>
        public ICollection<Node> TraversedNodes => _indices.Keys;

        /// <summary>
        /// Gets the index of the node during the traversal. 
        /// </summary>
        /// <param name="node">The node to get the index from.</param>
        /// <returns>The index.</returns>
        public int GetIndex(Node node)
        {
            return _indices.TryGetValue(node, out int index) ? index : -1;
        }

        public IList<Node> GetTraversal()
        {
            return _order.AsReadOnly();
        }
        
        private void TraversalOnNodeDiscovered(object sender, NodeDiscoveryEventArgs e)
        {
            if (!_indices.ContainsKey(e.NewNode))
            {
                _indices[e.NewNode] = _indices.Count;
                _order.Add(e.NewNode);
            }
        }
    }
}