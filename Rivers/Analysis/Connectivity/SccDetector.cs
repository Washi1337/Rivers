using System.Collections.Generic;
using System.Linq;
using Rivers.Analysis.Traversal;

namespace Rivers.Analysis.Connectivity
{
    /// <summary>
    /// Provides a mechanism for finding strongly connected components in a graph.
    /// </summary>
    public static class SccDetector
    {
        /// <summary>
        /// Finds all strongly connected components in the provided graph.
        /// </summary>
        /// <param name="graph">The graph to get the components from.</param>
        /// <returns>A collection of sets representing the strongly connected components.</returns>
        public static ICollection<ISet<Node>> FindStronglyConnectedComponents(this Graph graph)
        {
            return FindStronglyConnectedComponents(graph.Nodes.First());
        }

        public static ICollection<ISet<Node>> FindStronglyConnectedComponents(this Node entrypoint)
        {
            var graph = entrypoint.ParentGraph;
            
            var traversal = new DepthFirstTraversal();
            var recorder = new PostOrderRecorder(traversal);

            traversal.Run(entrypoint);

            var transpose = graph.Transpose();

            var visited = new HashSet<Node>();
            var result = new List<ISet<Node>>();
            foreach (var node in recorder.GetOrder().Reverse())
            {
                if (!visited.Contains(node))
                {
                    var subTraversal = new DepthFirstTraversal();
                    var component = new HashSet<Node>();
                    subTraversal.NodeDiscovered += (sender, args) =>
                    {
                        if (visited.Add(graph.Nodes[args.NewNode.Name]))
                        {
                            args.ContinueExploring = true;
                            component.Add(graph.Nodes[args.NewNode.Name]);
                        }
                    };
                    subTraversal.Run(transpose.Nodes[node.Name]);

                    result.Add(component);
                }
            }

            return result;
        }
    }
}