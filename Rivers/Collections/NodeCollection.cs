using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of nodes in a graph.
    /// </summary>
    public class NodeCollection : ICollection<Node>
    {
        private readonly IDictionary<string, Node> _nodes = new Dictionary<string, Node>();
        
        public NodeCollection(Graph parentGraph)
        {
            ParentGraph = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
        }
        
        /// <summary>
        /// Gets the parent graph containing the nodes.
        /// </summary>
        public Graph ParentGraph
        {
            get;
        }

        /// <summary>
        /// Gets a node by its name.
        /// </summary>
        /// <param name="name">The name of the node to get.</param>
        public Node this[string name]
        {
            get { return _nodes[name]; }
        }

        /// <inheritdoc />
        public int Count
        {
            get { return _nodes.Count; }
        }
        
        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool TryGetNode(string name, out Node node)
        {
            return _nodes.TryGetValue(name, out node);
        }

        /// <summary>
        /// Adds a new node with the given name if it is not present yet.
        /// </summary>
        /// <param name="name">The name of the node to add.</param>
        /// <returns>The node that was added to the graph, or the previously added node with the same name.</returns>
        public Node Add(string name)
        {
            if (!_nodes.TryGetValue(name, out var node))
            {
                node = new Node(name);
                Add(node);
            }

            return node;
        }
        
        /// <inheritdoc />
        public void Add(Node item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.ParentGraph != null && item.ParentGraph != ParentGraph)
                throw new ArgumentException("Node is already added to another graph.");

            if (!_nodes.ContainsKey(item.Name))
            {
                _nodes.Add(item.Name, item);
                item.ParentGraph = ParentGraph;
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var node in _nodes.Values)
                node.ParentGraph = null;
            _nodes.Clear();
        }

        /// <summary>
        /// Determines whether there exists in the collection a node with the povided name.
        /// </summary>
        /// <param name="nodeName">The name of the node to search.</param>
        /// <returns>True if a node exists, false otherwise.</returns>
        public bool Contains(string nodeName)
        {
            return _nodes.ContainsKey(nodeName);
        }

        /// <inheritdoc />
        public bool Contains(Node item)
        {
            if (item == null)
                return false;
            return _nodes.TryGetValue(item.Name, out var node) && node == item;
        }

        /// <inheritdoc />
        public void CopyTo(Node[] array, int arrayIndex)
        {
            _nodes.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a node with the provided name.
        /// </summary>
        /// <param name="nodeName">The name of the node to remove.</param>
        /// <returns>True if the node was removed, false otherwise.</returns>
        public bool Remove(string nodeName)
        {
           return TryGetNode(nodeName, out var node) && Remove(node);    
        }
        
        /// <inheritdoc />
        public bool Remove(Node item)
        {
            if (Contains(item))
            {
                // Remove all adjacent edges.
                foreach (var edge in item.GetEdges().ToArray())
                    ParentGraph.Edges.Remove(edge);

                // Remove node from subgraphs.
                if (item.SubGraphs.Count > 0)
                {
                    foreach (var subGraph in item.SubGraphs.ToArray())
                        subGraph.Nodes.Remove(item);
                }

                _nodes.Remove(item.Name);
                item.ParentGraph = null;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}