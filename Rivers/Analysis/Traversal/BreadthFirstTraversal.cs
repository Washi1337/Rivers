using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Traversal
{
    /// <summary>
    /// Represents a depth-first node traversal of a graph.
    /// </summary>
    public class BreadthFirstTraversal : ITraversal
    {
        /// <inheritdoc />
        public event EventHandler<NodeDiscoveryEventArgs> NodeDiscovered;

        public event EventHandler TraversalCompleted;

        /// <inheritdoc />
        public void Run(Node entrypoint)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<(Node node, Edge origin, int depth)>();
            queue.Enqueue((entrypoint, null, 0));

            while (queue.Count > 0)
            {
                var (node, origin, depth) = queue.Dequeue();

                var eventArgs = new NodeDiscoveryEventArgs(node, origin, depth)
                {
                    ContinueExploring = visited.Add(node)
                };
                OnNodeDiscovered(eventArgs);

                if (eventArgs.Abort)
                    return;

                if (eventArgs.ContinueExploring)
                {
                    foreach (var edge in node.OutgoingEdges)
                    {
                        queue.Enqueue((edge.GetOtherNode(node), edge, depth + 1));
                    }
                }
            }
            
            OnTraversalCompleted();
        }

        protected virtual void OnNodeDiscovered(NodeDiscoveryEventArgs e)
        {
            NodeDiscovered?.Invoke(this, e);
        }

        protected virtual void OnTraversalCompleted()
        {
            TraversalCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}