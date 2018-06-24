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
        /// Performs a traversal all descendants of a node in a breadth first order, and yields
        /// on every node it visits.
        /// </summary>
        /// <param name="start">The node in the graph to start the traversal at.</param>
        /// <returns>A lazy loaded ordered collection representing all nodes it traversed.</returns>
        public static IEnumerable<Node> BreadthFirstTraversal(this Node start)
        {
            var queue = new Queue<Node>();
            var visited = new HashSet<Node>();

            queue.Enqueue(start);
            
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                
                if (visited.Add(current))
                {
                    yield return current;
                    foreach (var edge in current.OutgoingEdges)
                        queue.Enqueue(edge.Target);
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
        /// Performs a traversal all descendants of a node in a depth first order, and yields
        /// on every node it visits.
        /// </summary>
        /// <param name="start">The node in the graph to start the traversal at.</param>
        /// <returns>A lazy loaded ordered collection containing all nodes it traversed.</returns>
        public static IEnumerable<Node> DepthFirstTraversal(this Node start, bool revisit = false)
        {
            var stack = new Stack<Node>();
            var visited = new HashSet<Node>();

            stack.Push(start);
            
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Add(current) || revisit)
                {
                    yield return current;
                    foreach (var successor in current.GetSuccessors())
                        stack.Push(successor);
                }
            }
        }

        public static bool IsCyclic(this Graph graph)
        {
            return graph.Nodes.Any(x => x.DepthFirstTraversal(true).Skip(1).Contains(x));
        }
        
    }
}