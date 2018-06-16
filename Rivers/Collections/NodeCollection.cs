using System;
using System.Collections;
using System.Collections.Generic;

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

        /// <inheritdoc />
        public bool Remove(Node item)
        {
            if (Contains(item))
            {
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