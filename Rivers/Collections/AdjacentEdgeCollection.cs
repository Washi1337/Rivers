using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of edges either originating, or targeting a node in a graph.
    /// </summary>
    public class AdjacentEdgeCollection : ICollection<Edge>
    {
        private readonly IDictionary<string, Edge> _edges = new Dictionary<string, Edge>();

        public AdjacentEdgeCollection(Node origin, bool outgoing)
        {
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            Outgoing = outgoing;
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains outgoing edges or incoming edges.
        /// </summary>
        public bool Outgoing
        {
            get;
        }
        
        /// <summary>
        /// Gets the node the adjacent edge collection is associated to.
        /// </summary>
        public Node Origin
        {
            get;
        }

        /// <inheritdoc />
        public int Count => _edges.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an edge based on the name of a neighbour.
        /// </summary>
        /// <param name="neighbour">The name of the neighbour.</param>
        public Edge this[string neighbour] => _edges[neighbour];

        /// <summary>
        /// Tries to get an edge based on the name of a neighbour.
        /// </summary>
        /// <param name="neighbour">The name of the neighbour.</param>
        /// <param name="edge">The edge either originating, or going to the provided neighbour.</param>
        /// <returns>True if the edge is present, false otherwise.</returns>
        public bool TryGetEdge(string neighbour, out Edge edge)
        {
            return _edges.TryGetValue(neighbour, out edge);
        }
        
        /// <summary>
        /// Adds a new edge to the node, either originating or towards a new neighbour, depending on the value of <see cref="Outgoing"/>.
        /// </summary>
        /// <param name="neighbour">The name of the new neighbour.</param>
        public void Add(string neighbour)
        {
            Add(Origin.ParentGraph.Nodes[neighbour]);
        }

        /// <summary>
        /// Adds a new edge to the node, either originating or towards a new neighbour, depending on the value of <see cref="Outgoing"/>.
        /// </summary>
        /// <param name="neighbour">The new neighbour node.</param>
        public void Add(Node neighbour)
        {
            if (neighbour == null)
                throw new ArgumentNullException(nameof(neighbour));
            
            if (Outgoing)
                Add(new Edge(Origin, neighbour));
            else
                Add(new Edge(neighbour, Origin));
        }

        /// <summary>
        /// Adds a new edge to the node.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <exception cref="ArgumentNullException">Occurs when the edge is null.</exception>
        /// <exception cref="ArgumentException">Occurs when the edge does not conform to the value in <see cref="Outgoing"/>.</exception>
        public void Add(Edge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            
            if (Outgoing && edge.Source != Origin)
                throw new ArgumentException("Edge must be originating from the origin.");
            else if (!Outgoing && edge.Target != Origin)
                throw new ArgumentException("Edge must have a target equal to the origin.");

            var neighbour = Outgoing ? edge.Target : edge.Source;

            if (!_edges.ContainsKey(neighbour.Name))
            {
                _edges.Add(neighbour.Name, edge);
                if (Outgoing)
                    neighbour.IncomingEdges.Add(edge);
                else
                    neighbour.OutgoingEdges.Add(edge);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var item in _edges.Values)
            {
                if (Outgoing)
                    item.Target.IncomingEdges.Remove(item);
                else
                    item.Source.OutgoingEdges.Remove(item);
            }
            
            _edges.Clear();
        }

        /// <summary>
        /// Determines whether there exists an edge from/to the given node, depending on the value of <see cref="Outgoing"/>.   
        /// </summary>
        /// <param name="neighbour">The neighbour.</param>
        /// <returns>True if there exists an edge, false otherwise.</returns>
        public bool Contains(string neighbour)
        {
            return _edges.ContainsKey(neighbour);
        }

        /// <inheritdoc />
        public bool Contains(Edge item)
        {
            if (item == null)
                return false;
            return _edges.ContainsKey((Outgoing ? item.Target : item.Source).Name);
        }

        /// <inheritdoc />
        public void CopyTo(Edge[] array, int arrayIndex)
        {
            _edges.Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(Edge item)
        {
            if (item == null)
                return false;
            
            var neighbour = Outgoing ? item.Target : item.Source; 
            if (_edges.ContainsKey(neighbour.Name))
            {
                _edges.Remove(neighbour.Name);
                if (Outgoing)
                    item.Target.IncomingEdges.Remove(item);
                else
                    item.Source.OutgoingEdges.Remove(item);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerator<Edge> GetEnumerator()
        {
            return _edges.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}