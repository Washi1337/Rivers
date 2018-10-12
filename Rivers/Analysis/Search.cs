using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis
{
    /// <summary>
    /// Provides a collection of standard graph search algorithms.
    /// </summary>
    public static class Search
    {
        public delegate bool ContinuationDelegate(Node origin, Edge edgeToTraverse);
        
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
        
        public static IEnumerable<Node> PreOrderTraversal(this Node start)
        {
            yield return start;
            
            foreach (var successor in start.GetSuccessors())
            {
                foreach (var node in successor.PreOrderTraversal())
                    yield return node;
            }
        }

        public static IEnumerable<Node> PostOrderTraversal(this Node start)
        {
            foreach (var successor in start.GetSuccessors())
            {
                foreach (var node in successor.PostOrderTraversal())
                    yield return node;
            }

            yield return start;
        }
        
        /// <summary>
        /// Performs a search for a node in a graph in a breadth first order.
        /// </summary>
        /// <param name="start">The node in the graph to start the search at.</param>
        /// <param name="predicate">The predicate the to-be-searched node needs to satisfy.</param>
        /// <returns>The found node, or <c>null</c> if none was found.</returns>
        public static Node BreadthFirstSearch(this Node start, Predicate<Node> predicate)
        {
            return BreadthFirstTraversal(start).FirstOrDefault(new Func<Node, bool>(predicate));
        }

        /// <summary>
        /// Performs a traversal of all descendants of a node in a breadth first order, and yields
        /// on every node it visits.
        /// </summary>
        /// <param name="start">The node in the graph to start the traversal at.</param>
        /// <returns>A lazy loaded ordered collection representing all nodes it traversed.</returns>
        public static IEnumerable<Node> BreadthFirstTraversal(this Node start, bool revisit = false)
        {
            var visited = new HashSet<Node> {start};
            return BreadthFirstTraversal(start, (n, e) => visited.Add(e.GetOtherNode(n)) || revisit);
        }

        /// <summary>
        /// Performs a traversl of all descendants of a node in a breadth first order, and yields
        /// on every node it visits. An edge is traversed when the given condition is met.
        /// </summary>
        /// <param name="start">The node in the graph to start the travesal at.</param>
        /// <param name="continueCondition">The condition an edge has to met in order to be traversed.</param>
        /// <returns></returns>
        public static IEnumerable<Node> BreadthFirstTraversal(this Node start, ContinuationDelegate continueCondition)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(start);
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;
                foreach (var edge in current.OutgoingEdges)
                {
                    if (continueCondition(current, edge))
                        queue.Enqueue(edge.GetOtherNode(current));
                }
            }
        }
        
        /// <summary>
        /// Performs a search for a node in a graph in a depth first order.
        /// </summary>
        /// <param name="start">The node in the graph to start the search at.</param>
        /// <param name="predicate">The predicate the to-be-searched node needs to satisfy.</param>
        /// <returns>The found node, or <c>null</c> if none was found.</returns>
        public static Node DepthFirstSearch(this Node start, Predicate<Node> predicate)
        {
            return DepthFirstTraversal(start).FirstOrDefault(new Func<Node, bool>(predicate));
        }

        /// <summary>
        /// Performs a traversal on all descendants of a node in a depth first order, and yields
        /// on every node it visits.
        /// </summary>
        /// <param name="start">The node in the graph to start the traversal at.</param>
        /// <returns>A lazy loaded ordered collection containing all nodes it traversed.</returns>
        public static IEnumerable<Node> DepthFirstTraversal(this Node start, bool revisit = false)
        {
            var visited = new HashSet<Node> {start};
            return DepthFirstTraversal(start, (n, e) => visited.Add(e.GetOtherNode(n)) || revisit);
        }

        /// <summary>
        /// Performs a traversal on all descendants of a node in a depth first order, and yields
        /// on every node it visits. 
        /// </summary>
        /// <param name="start">The node in the graph to start the travesal at.</param>
        /// <param name="continueCondition">The condition an edge has to met in order to be traversed.</param>
        /// <returns>A lazy loaded ordered collection containing all nodes it traversed.</returns>
        public static IEnumerable<Node> DepthFirstTraversal(this Node start, ContinuationDelegate continueCondition)
        {           
            var stack = new Stack<Node>();
            stack.Push(start);
            
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                yield return current;
                foreach (var edge in current.OutgoingEdges)
                {
                    if (continueCondition(current, edge))
                        stack.Push(edge.GetOtherNode(current));
                }
            }            
        }
    }
}