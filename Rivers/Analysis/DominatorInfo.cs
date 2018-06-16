using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis
{
    /// <summary>
    /// Provides information about dominators in control flow graphs.
    /// </summary>
    public class DominatorInfo
    {
        private readonly IDictionary<Node, Node> _dominators;
        private readonly IDictionary<Node, ISet<Node>> _frontiers;

        /// <summary>
        /// Collects all dominator information from control flow graph, defined by its entrypoint.
        /// </summary>
        /// <param name="entrypoint"></param>
        public DominatorInfo(Node entrypoint)
        {
            _dominators = GetDominatorTree(entrypoint);
            _frontiers = GetDominanceFrontier(entrypoint.ParentGraph, _dominators);
        }

        /// <summary>
        /// Gets the immediate dominator of a node in the analysed control flow graph. 
        /// </summary>
        /// <param name="node">The node to get the immediate dominator from.</param>
        /// <returns>The immediate dominator, or <c>null</c> if the provided node was not
        /// present upon analysing the graph.</returns>
        public Node GetImmediateDominator(Node node)
        {
            _dominators.TryGetValue(node, out var dominator);
            return dominator;
        }

        /// <summary>
        /// Gets a collection containing all nodes for which the dominance of the provided starting node ends.   
        /// </summary>
        /// <param name="node">The node to get the frontier of.</param>
        /// <returns>A set of nodes representing the dominance frontier, or <c>null</c> if the provided node was
        /// not present upon analysing the graph.</returns>
        public ISet<Node> GetDominanceFrontier(Node node)
        {
            _frontiers.TryGetValue(node, out var frontier);
            return frontier;
        }

        /// <summary>
        /// Collects all the nodes being dominated by a starting node.
        /// </summary>
        /// <param name="node">The starting node.</param>
        /// <returns>A set of all nodes dominated by the starting node.</returns>
        public ISet<Node> GetDominatedNodes(Node node)
        {
            var nodes = new HashSet<Node>();
            var agenda = new Stack<Node>();
            agenda.Push(node);

            while (agenda.Count > 0)
            {
                var current = agenda.Pop();
                if (nodes.Add(current))
                {
                    foreach (var s in current.GetSuccessors())
                    {
                        if (!_frontiers[node].Contains(s))
                            agenda.Push(s);
                    }
                }
            }

            return nodes;
        }
      
        /// <summary>
        /// Computes the dominator tree of a control flow graph, defined by its entrypoint.
        /// </summary>
        /// <param name="entrypoint">The entrypoint of the control flow graph.</param>
        /// <returns>A dictionary mapping all the nodes to their immediate dominator.</returns>
        /// <remarks>
        /// Algorithm is based on the paper:
        ///     Keith D. Cooper, Timothy J. Harvey, and Ken Kennedy
        ///     A Simple, Fast Dominance Algorithm
        ///     https://www.cs.rice.edu/~keith/Embed/dom.pdf (Accessed on June 2018)
        /// </remarks>
        private static IDictionary<Node, Node> GetDominatorTree(Node entrypoint)
        {
            var dominators = entrypoint.ParentGraph.Nodes.ToDictionary(x => x, null);
            dominators[entrypoint] = entrypoint;

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var node in entrypoint.ParentGraph.Nodes)
                {
                    if (node == entrypoint)
                        continue;

                    var first = node.GetPredecessors().First();
                    var newIdom = first;
                    foreach (var otherPred in node.GetPredecessors())
                    {
                        if (otherPred == first)
                            continue;
                        if (dominators[otherPred] != null)
                            newIdom = Intersect(dominators, otherPred, newIdom);
                    }

                    if (dominators[node] != newIdom)
                    {
                        dominators[node] = newIdom;
                        changed = true;
                    }
                }
            }

            return dominators;
        }

        /// <summary>
        /// Finds the first common ancestor of two nodes in the dominator tree.
        /// </summary>
        /// <param name="dominators">The immediate dominators of each node.</param>
        /// <param name="node1">The first node.</param>
        /// <param name="node2">The second node.</param>
        /// <returns>The first common ancestor.</returns>
        private static Node Intersect(IDictionary<Node, Node> dominators, Node node1, Node node2)
        {
            var hashSet = new HashSet<Node>();
            while (node1 != null)
            {
                if (!hashSet.Add(node1))
                    break;
                node1 = dominators[node1];
            }
            while (node2 != null)
            {
                if (!hashSet.Add(node2))
                    return node2;
                node2 = dominators[node2];
            }
            return null;
        }
        
        /// <summary>
        /// Computes the dominance frontiers of all nodes in the provided control flow graph.
        /// </summary>
        /// <param name="graph">The control flow graph.</param>
        /// <param name="dominators">A dictionary mapping all nodes to their immediate dominators.</param>
        /// <returns>A dictionary mapping all nodes to their associated dominance frontiers.</returns>
        private static IDictionary<Node, ISet<Node>> GetDominanceFrontier(Graph graph, IDictionary<Node, Node> dominators)
        {
            var frontier = graph.Nodes.ToDictionary(x => x, x => (ISet<Node>) new HashSet<Node>());
            
            foreach (var node in graph.Nodes)
            {
                var predecessors = node.GetPredecessors().ToArray();
                if (predecessors.Length >= 2)
                {
                    foreach (var p in predecessors)
                    {
                        var runner = p;
                        while (runner != dominators[node])
                        {
                            frontier[runner].Add(node);
                            runner = dominators[runner];
                        }
                    }
                }
            }

            return frontier;
        }

    }
}