using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    public static class Traversal
    {
        /// <summary>
        /// Obtains a collection of nodes that is reachable from the provided node.
        /// </summary>
        /// <param name="start">The node to start the search at.</param>
        /// <returns>The nodes that were found reachable.</returns>
        public static ICollection<Node> GetReachableNodes(this Node start)
        {
            return GetReachableNodes(start, new BreadthFirstTraversal());
        }
        
        /// <summary>
        /// Obtains a collection of nodes that is reachable from the provided node.
        /// </summary>
        /// <param name="start">The node to start the search at.</param>
        /// <param name="traversal">The type of traversal to use to discover the reachable nodes.</param>
        /// <returns>The nodes that were found reachable.</returns>
        public static ICollection<Node> GetReachableNodes(this Node start, ITraversal traversal)
        {
            var recorder = new TraversalRecorder(traversal);
            traversal.Run(start);
            return recorder.TraversedNodes;
        }
        
        /// <summary>
        /// Performs a search on a node in a graph using the provided traversal method.
        /// </summary>
        /// <param name="start">The node to start the search on.</param>
        /// <param name="traversal">The type of traversal to perform.</param>
        /// <param name="predicate">The predicate the to-be-searched node needs to satisfy.</param>
        /// <returns>The first node matching the provided predicate, or <c>null</c> if none was found.</returns>
        public static Node Search(this Node start, ITraversal traversal, Predicate<Node> predicate)
        {
            Node result = null;
            traversal.NodeDiscovered += (sender, args) =>
            {
                if (predicate(args.NewNode))
                {
                    args.ContinueExploring = false;
                    args.Abort = true;
                    result = args.NewNode;
                }
            };
            traversal.Run(start);
            return result;
        }
        
        /// <summary>
        /// Performs a search for a node in a graph in a breadth first order.
        /// </summary>
        /// <param name="start">The node in the graph to start the search at.</param>
        /// <param name="predicate">The predicate the to-be-searched node needs to satisfy.</param>
        /// <returns>The first node matching the provided predicate, or <c>null</c> if none was found.</returns>
        public static Node BreadthFirstSearch(this Node start, Predicate<Node> predicate)
        {
            return start.Search(new BreadthFirstTraversal(), predicate);
        }
        
        /// <summary>
        /// Performs a search for a node in a graph in a depth first order.
        /// </summary>
        /// <param name="start">The node in the graph to start the search at.</param>
        /// <param name="predicate">The predicate the to-be-searched node needs to satisfy.</param>
        /// <returns>The first node matching the provided predicate, or <c>null</c> if none was found.</returns>
        public static Node DepthFirstSearch(this Node start, Predicate<Node> predicate)
        {
            return start.Search(new DepthFirstTraversal(), predicate);
        }
    }
}