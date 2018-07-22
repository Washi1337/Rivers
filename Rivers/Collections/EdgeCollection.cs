using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers.Collections
{
    public abstract class EdgeCollection : ICollection<Edge>
    {
        protected EdgeCollection(Graph parentGraph)
        {
            ParentGraph = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
        }
        
        /// <summary>
        /// Gets the graph that contains the collection of edges.
        /// </summary>
        public Graph ParentGraph
        {
            get;
        }

        /// <summary>
        /// Gets an edge by its source and target node.
        /// </summary>
        /// <param name="source">The name of the source node.</param>
        /// <param name="target">The name of the target node.</param>
        /// <exception cref="KeyNotFoundException">Occurs when the edge does not exist in the graph.</exception>
        public Edge this[string source, string target] =>
            this[ParentGraph.Nodes[source], ParentGraph.Nodes[target]];
        
        /// <summary>
        /// Gets an edge by its source and target node.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <exception cref="KeyNotFoundException">Occurs when the edge does not exist in the graph.</exception>
        public Edge this[Node source, Node target]
        {
            get
            {
                if (!TryGetEdge(source, target, out var edge))
                    throw new KeyNotFoundException();
                return edge;
            }
        }

        /// <inheritdoc />
        public abstract int Count { get; }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        public virtual bool TryGetEdge(string sourceName, string targetName, out Edge edge)
        {
            if (ParentGraph.Nodes.TryGetNode(sourceName, out var sourceNode)
                && ParentGraph.Nodes.TryGetNode(targetName, out var targetNode))
            {
                return TryGetEdge(sourceNode, targetNode, out edge);
            }
            
            edge = null;
            return false;
        }
        
        public virtual bool TryGetEdge(Node sourceNode, Node targetNode, out Edge edge)
        {
            return sourceNode.OutgoingEdges.TryGetEdge(targetNode, out edge);
        }

        /// <summary>
        /// Adds an edge to the graph between two nodes.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>The edge that was created, or the already existing edge.</returns>
        public Edge Add(string source, string target)
        {
            return Add(ParentGraph.Nodes[source], ParentGraph.Nodes[target]);
        }

        /// <summary>
        /// Adds an edge to the graph between two nodes.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>The edge that was created, or the already existing edge.</returns>
        public Edge Add(Node source, Node target)
        {
            var edge = new Edge(source, target);
            Add(edge);
            return edge;
        }

        void ICollection<Edge>.Add(Edge edge)
        {
            Add(edge);
        }

        /// <summary>
        /// Adds an edge to the graph between two nodes.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>The edge that was created, or the already existing edge.</returns>
        public abstract Edge Add(Edge edge);

        /// <inheritdoc />
        public abstract void Clear();

        /// <inheritdoc />
        public abstract bool Contains(Edge item);

        /// <inheritdoc />
        public abstract void CopyTo(Edge[] array, int arrayIndex);

        /// <inheritdoc />
        public abstract bool Remove(Edge item);

        /// <inheritdoc />
        public abstract IEnumerator<Edge> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}