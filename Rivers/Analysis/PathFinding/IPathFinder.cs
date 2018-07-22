using System.Collections.Generic;

namespace Rivers.Analysis.PathFinding
{
    /// <summary>
    /// Provides members for finding a path between two nodes in a graph.
    /// </summary>
    public interface IPathFinder
    {
        /// <summary>
        /// Finds a path from the provided source node to the given destination node.
        /// </summary>
        /// <param name="source">The node to start the path at.</param>
        /// <param name="destination">The node to reach.</param>
        /// <returns>A sequence of nodes representing the found path. This path includes the source and destination nodes.</returns>
        IList<Node> FindPath(Node source, Node destination);
    }
}