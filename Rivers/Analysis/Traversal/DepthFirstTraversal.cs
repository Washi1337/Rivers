using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    /// <summary>
    /// Represents a depth-first node traversal of a graph.
    /// </summary>
    public class DepthFirstTraversal : ITraversal
    {
        /// <inheritdoc />
        public event EventHandler<NodeDiscoveryEventArgs> NodeDiscovered;
        
        /// <inheritdoc />
        public void Run(Node entrypoint)
        {
            if (entrypoint == null)
                throw new ArgumentNullException(nameof(entrypoint));
            
            var visited = new HashSet<Node>();
            var stack = new Stack<(Node node, Edge edge)>();
            stack.Push((entrypoint, null));
            
            while (stack.Count > 0)
            {
                var (node, origin) = stack.Pop();
                var eventArgs = new NodeDiscoveryEventArgs(node, origin)
                {
                    ContinueExploring = visited.Add(node)
                };
                OnNodeDiscovered(eventArgs);

                if (eventArgs.Abort)
                    return;
                
                if (eventArgs.ContinueExploring)
                {
                    foreach (var edge in node.OutgoingEdges)
                        stack.Push((edge.GetOtherNode(node), edge));
                }
            }
        }

        protected virtual void OnNodeDiscovered(NodeDiscoveryEventArgs e)
        {
            NodeDiscovered?.Invoke(this, e);
        }
    }
}