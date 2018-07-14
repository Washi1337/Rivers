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
        /// <param name="graph">The graph to check.</param>
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
        /// <param name="graph">The graph to check.</param>
        /// <returns>True if the graph is (weakly) connected, false otherwise.</returns>
        public static bool IsConnected(this Graph graph)
        {
            if (graph.IsDirected)
                graph = graph.ToUndirected();
            return graph.Nodes.First().BreadthFirstTraversal().Count() == graph.Nodes.Count;
        }
        
        /// <summary>
        /// Determines whether a graph is a tree.
        /// </summary>
        /// <param name="graph">The graph to check.</param>
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

    }
}