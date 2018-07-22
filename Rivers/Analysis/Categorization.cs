using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis
{
    public static class Categorization
    {
        /// <summary>
        /// Determines whether a graph contains at least one cycle.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns>True if the graph is cyclic, false otherwise.</returns>
        public static bool IsCyclic(this Graph graph)
        {
            var visited = new HashSet<Node>();
            foreach (var node in graph.Nodes)
            {
                if (!visited.Add(node))
                    return false;

                var reachableNodes = new HashSet<Node>();
                var visitedEdges = new HashSet<Edge>();

                if (node.DepthFirstTraversal((n, e) => visitedEdges.Add(e))
                    .Any(successor => !reachableNodes.Add(successor)))
                    return true;

                visited.UnionWith(reachableNodes);
            }

            return false;
        }

        /// <summary>
        /// Determines whether a graph is connected. That is, every vertex is reachable from any other vertex.
        /// For directed graphs this method verifies whether the graph is weakly connected.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns>True if the graph is (weakly) connected, false otherwise.</returns>
        public static bool IsConnected(this Graph graph)
        {
            if (graph.Nodes.Count <= 1)
                return true;
            
            if (graph.IsDirected)
                graph = graph.ToUndirected();
            
            return graph.Nodes.First().BreadthFirstTraversal().Count() == graph.Nodes.Count;
        }
        
        /// <summary>
        /// Determines whether a graph is a tree. That is, a (weakly) connected acyclic graph.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns>True if the graph is a tree, false otherwise.</returns>
        public static bool IsTree(this Graph graph)
        {
            var node = graph.Nodes.First();
            int count = 1;
            
            var visitedEdges = new HashSet<Edge>();
            foreach (var descendant in node.BreadthFirstTraversal((n, e) => visitedEdges.Add(e)).Skip(1))
            {
                if (descendant == node)
                    return false;
                count++;
            }

            return count == graph.Nodes.Count;
        }

        /// <summary>
        /// Determines whether a graph is regular. That is, every node in the graph has the same degree.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns>True if the graph is regular, false otherwise.</returns>
        public static bool IsRegular(this Graph graph)
        {
            if (graph.Nodes.Count <= 1)
                return true;
            var firstNode = graph.Nodes.First();
            return graph.Nodes.All(n => n.InDegree == firstNode.InDegree && n.OutDegree == firstNode.OutDegree);
        }

        /// <summary>
        /// Determines whether a graph is complete. That is, every node in the graph is connected to every other
        /// node in the graph.
        /// </summary>
        /// <param name="graph">The graph to test.</param>
        /// <returns>True if the graph is complete, false otherwise.</returns>
        public static bool IsComplete(this Graph graph)
        {
            foreach (var n1 in graph.Nodes)
            {
                if (graph.Nodes
                    .Where(x => x != n1)
                    .Any(n2 => !n1.OutgoingEdges.Contains(n2) || !n2.OutgoingEdges.Contains(n1)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}