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

        /// <inheritdoc />
        public abstract int Count { get; }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        public virtual bool TryGetEdge(string sourceName, string targetName, out Edge edge)
        {
            if (ParentGraph.Nodes.TryGetNode(sourceName, out var sourceNode)
                && ParentGraph.Nodes.TryGetNode(targetName, out var targetNode))
            {
                if (sourceNode.OutgoingEdges.TryGetEdge(targetName, out edge))
                    return true;
            }
            
            edge = null;
            return false;
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