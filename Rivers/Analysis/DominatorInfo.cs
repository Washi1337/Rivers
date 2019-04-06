using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rivers.Analysis.Traversal;

namespace Rivers.Analysis
{
    /// <summary>
    /// Provides information about dominators in control flow graphs.
    /// </summary>
    public class DominatorInfo
    {
        private readonly Node _entrypoint;
        private readonly IDictionary<Node, Node> _dominators;
        private readonly IDictionary<Node, ISet<Node>> _frontiers;

        /// <summary>
        /// Collects all dominator information from control flow graph, defined by its entrypoint.
        /// </summary>
        /// <param name="entrypoint"></param>
        public DominatorInfo(Node entrypoint)
        {
            _entrypoint = entrypoint;
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
        /// Constructs a dominator tree from the control flow graph.
        /// </summary>
        /// <returns>The constructed tree. Each node added to the tree is linked to a node in the original graph by
        /// its name.</returns>
        public Graph ToDominatorTree()
        {
            var tree = new Graph();
            tree.Nodes.Add(_entrypoint.Name);
            
            foreach (var entry in _dominators)
            {
                var dominator = entry.Value;
                var dominated = entry.Key;

                if (dominator != dominated)
                {
                    var child = tree.Nodes.Add(dominated.Name);
                    var parent = tree.Nodes.Add(dominator.Name);
                    tree.Edges.Add(parent, child);
                }
            }

            return tree;
        }

        /// <summary>
        /// Computes the dominator tree of a control flow graph, defined by its entrypoint.
        /// </summary>
        /// <param name="entrypoint">The entrypoint of the control flow graph.</param>
        /// <returns>A dictionary mapping all the nodes to their immediate dominator.</returns>
        /// <remarks>
        /// The algorithm used is based on the one engineered by Lengauer and Tarjan.
        /// https://www.cs.princeton.edu/courses/archive/fall03/cs528/handouts/a%20fast%20algorithm%20for%20finding.pdf
        /// https://www.cl.cam.ac.uk/~mr10/lengtarj.pdf
        /// </remarks> 
        private static IDictionary<Node, Node> GetDominatorTree(Node entrypoint)
        {
            var idom = new Dictionary<Node, Node>();
            var semi = new Dictionary<Node, Node>();
            var ancestor = new Dictionary<Node, Node>();
            var bucket = new Dictionary<Node, ISet<Node>>();

            var traversal = new DepthFirstTraversal();
            var order = new TraversalOrderRecorder(traversal);
            var parents = new ParentRecorder(traversal);
            traversal.Run(entrypoint);

            var orderedNodes = order.GetTraversal();            
            foreach (var node in orderedNodes)
            {
                idom[node] = null;
                semi[node] = node;
                ancestor[node] = null;
                bucket[node] = new HashSet<Node>();
            }

            for (int i = orderedNodes.Count - 1; i >= 1; i--)
            {
                var current = orderedNodes[i];
                var parent = parents.GetParent(current);

                // step 2
                foreach (var predecessor in current.GetPredecessors())
                {
                    var u = Eval(predecessor, ancestor, semi, order);
                    if (order.GetIndex(semi[current]) > order.GetIndex(semi[u]))
                        semi[current] = semi[u];
                }

                bucket[semi[current]].Add(current);
                Link(parent, current, ancestor);
                
                // step 3
                foreach (var bucketNode in bucket[parent])
                {
                    var u = Eval(bucketNode, ancestor, semi, order);
                    if (order.GetIndex(semi[u]) < order.GetIndex(semi[bucketNode]))
                        idom[bucketNode] = u;
                    else
                        idom[bucketNode] = parent;
                }

                bucket[parent].Clear();
            }

            // step 4
            for (int i = 1; i < orderedNodes.Count; i++)
            {
                var w = orderedNodes[i];
                if (idom[w] != semi[w])
                    idom[w] = idom[idom[w]];
            }

            idom[entrypoint] = entrypoint;
            return idom;
        }

        private static void Link(Node parent, Node node, IDictionary<Node, Node> ancestors)
        {
            ancestors[node] = parent;
        }

        private static Node Eval(Node node, IDictionary<Node, Node> ancestors, IDictionary<Node, Node> semi, TraversalOrderRecorder order)
        {
            var a = ancestors[node];
            while (a != null && ancestors[a] != null)
            {
                if (order.GetIndex(semi[node]) > order.GetIndex(semi[a]))
                    node = a;
                a = ancestors[a];
            }

            return node;
        }
        
        /// <summary>
        /// Computes the dominance frontiers of all nodes in the provided control flow graph.
        /// </summary>
        /// <param name="graph">The control flow graph.</param>
        /// <param name="dominators">A dictionary mapping all nodes to their immediate dominators.</param>
        /// <returns>A dictionary mapping all nodes to their associated dominance frontiers.</returns>
        private static IDictionary<Node, ISet<Node>> GetDominanceFrontier(Graph graph, IDictionary<Node, Node> dominators)
        {
            var frontier = dominators.Keys.ToDictionary(x => x, x => (ISet<Node>) new HashSet<Node>());
            
            foreach (var node in dominators.Keys)
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

        /// <summary>
        /// Determines whether a node is dominated by another node.
        /// </summary>
        /// <param name="dominator">The node to check for being the dominator.</param>
        /// <param name="dominated">The dominated node.</param>
        /// <returns>True if the dominator actually dominates the node, false otherwise.</returns>
        public bool Dominates(Node dominator, Node dominated)
        {
            var current = dominated;
            while (true)
            {
                if (current == dominator)
                    return true;
                var node = GetImmediateDominator(current);
                if (node == current)
                    return false;
                current = node;
            }
        }

        /// <summary>
        /// Determines whether an edge is a back-edge or not. That is, whether its target dominates its source. 
        /// </summary>
        /// <param name="edge">The edge to check.</param>
        /// <returns>True if the edge is a back-edge, false otherwise.</returns>
        public bool IsBackEdge(Edge edge)
        {
            return Dominates(edge.Target, edge.Source);
        }
        
        /// <summary>
        /// Determines whether a node is a loop header or not. That is, whether it is a target of any back-edge.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>True if the node is a loop header, false otherwise.</returns>
        public bool IsLoopHeader(Node node)
        {
            return node.IncomingEdges.Any(IsBackEdge);
        }
    }
}