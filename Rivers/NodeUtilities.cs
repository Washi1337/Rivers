using System.Collections.Generic;
using System.Linq;

namespace Rivers
{
    /// <summary>
    /// Provides a collection of standard graph search algorithms.
    /// </summary>
    public static class NodeUtilities
    {
        /// <summary>
        /// Gets a collection of predecessor nodes, given by the incoming edges of the given node.
        /// </summary>
        /// <param name="node">The node to get the predecessors from.</param>
        /// <returns>A collection of predecessor nodes.</returns>
        public static IEnumerable<Node> GetPredecessors(this Node node)
        {
            return node.IncomingEdges.Select(x => x.Source);
        }

        /// <summary>
        /// Gets a collection of successor nodes, given by the outgoing edges of the given node.
        /// </summary>
        /// <param name="node">The node to get the successors from.</param>
        /// <returns>A collection of successor nodes.</returns>
        public static IEnumerable<Node> GetSuccessors(this Node node)
        {
            return node.OutgoingEdges.Select(x => x.Target);
        }
        
        /// <summary>
        /// Gets a collection of neighbouring nodes.
        /// </summary>
        public static IEnumerable<Node> GetNeighbours(this Node node)
        {
            return node.GetPredecessors().Union(node.GetSuccessors());
        }
        
    }
}