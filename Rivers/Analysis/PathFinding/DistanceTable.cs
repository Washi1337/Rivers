using System;
using System.Collections.Generic;

namespace Rivers.Analysis.PathFinding
{
    /// <summary>
    /// Represents a distance table associated to a node in a graph, containing the distances to all nodes in the graph. 
    /// </summary>
    public class DistanceTable
    {
        public DistanceTable(Node source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Distances = new Dictionary<Node, double>
            {
                [source] = 0,
            };

            Previous = new Dictionary<Node, Node>
            {
              //  [source] = source
            };
        }

        /// <summary>
        /// Gets the node the table is associated to.
        /// </summary>
        public Node Source
        {
            get;
        }
        
        /// <summary>
        /// Gets a mapping containing the distances between the source node and every other node in the graph. 
        /// </summary>
        public IDictionary<Node, double> Distances
        {
            get;
        }

        /// <summary>
        /// Gets a mapping where each entry contains a value representing the previous node in the (optimal) path to
        /// the node stored in the key.
        /// </summary>
        public IDictionary<Node, Node> Previous
        {
            get;
        }

        /// <summary>
        /// Obtains the (optimal) path from the source node to the given target node.
        /// </summary>
        /// <param name="target">The destination node.</param>
        /// <returns>A sequence of nodes representing the obtained path. This path includes both the source and destination nodes.</returns>
        public IList<Node> GetShortestPath(Node target)
        {
            var path = new List<Node>();

            while (Previous.TryGetValue(target, out var previous) && previous != null)
            {
                path.Insert(0, target);
                target = previous;
            }

            if (Source != target)
                return null;
            
            path.Insert(0, Source);
            return path;
        }
    }
}