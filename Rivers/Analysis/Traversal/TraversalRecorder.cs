using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    /// <summary>
    /// Provides a mechanism for recording all discovered nodes during a traversal.
    /// </summary>
    public class TraversalRecorder
    {
        private readonly List<Node> _nodes = new List<Node>();
        private readonly ITraversal _traversal;

        public TraversalRecorder(ITraversal traversal)
        {
            _traversal = traversal ?? throw new ArgumentNullException(nameof(traversal));
            _traversal.NodeDiscovered += (sender, args) =>
            {
                TraversedNodes.Add(args.NewNode);
                _nodes.Add(args.NewNode);
            };
        }

        /// <summary>
        /// Gets a collection of all traversed nodes.
        /// </summary>
        public ISet<Node> TraversedNodes
        {
            get;
        } = new HashSet<Node>();
        
        /// <summary>
        /// Gets an ordered list of nodes in which the traversal algorithm found the nodes.
        /// </summary>
        public IList<Node> Traversal => _nodes.AsReadOnly();
    }
}