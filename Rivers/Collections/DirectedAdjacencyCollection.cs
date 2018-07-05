using System;
using System.Collections.Generic;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of edges either originating, or targeting a node in a directed graph.
    /// </summary>
    public class DirectedAdjacencyCollection : AdjacencyCollection
    {
        private readonly IDictionary<string, Edge> _edges = new Dictionary<string, Edge>();

        public DirectedAdjacencyCollection(Node origin, bool outgoing)
            : base(origin)
        {
            Outgoing = outgoing;
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains outgoing edges or incoming edges.
        /// </summary>
        public bool Outgoing
        {
            get;
        }

        /// <inheritdoc />
        public override int Count => _edges.Count;

        /// <inheritdoc />
        public override Edge this[string neighbour] => _edges[neighbour];
        
        /// <inheritdoc />
        public override Edge this[Node neighbour] => _edges[neighbour.Name];

        /// <inheritdoc />
        public override bool TryGetEdge(string neighbour, out Edge edge)
        {
            return _edges.TryGetValue(neighbour, out edge);
        }
        
        /// <inheritdoc />
        public override bool TryGetEdge(Node neighbour, out Edge edge)
        {
            return _edges.TryGetValue(neighbour.Name, out edge) &&
                   ReferenceEquals((Outgoing ? edge.Target : edge.Source), neighbour);
        }
        
        /// <inheritdoc />
        public override void Add(string neighbour)
        {
            Add(Origin.ParentGraph.Nodes[neighbour]);
        }

        /// <inheritdoc />
        public override void Add(Node neighbour)
        {
            if (neighbour == null)
                throw new ArgumentNullException(nameof(neighbour));
            
            if (Outgoing)
                Add(new Edge(Origin, neighbour));
            else
                Add(new Edge(neighbour, Origin));
        }

        /// <inheritdoc />
        public override void Add(Edge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            
            if (Outgoing && edge.Source != Origin)
                throw new ArgumentException("Edge must be originating from the origin.");
            if (!Outgoing && edge.Target != Origin)
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
        public override void Clear()
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

        /// <inheritdoc />
        public override bool Contains(string neighbour)
        {
            return neighbour != null && _edges.ContainsKey(neighbour);
        }

        /// <inheritdoc />
        public override bool Contains(Node neighbour)
        {
            return TryGetEdge(neighbour, out _);
        }

        /// <inheritdoc />
        public override bool Contains(Edge item)
        {
            return item != null && _edges.ContainsKey((Outgoing ? item.Target : item.Source).Name);
        }
        
        /// <inheritdoc />
        public override void CopyTo(Edge[] array, int arrayIndex)
        {
            _edges.Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public override bool Remove(string neighbourName)
        {
            return Contains(neighbourName) && Remove(_edges[neighbourName]);
        }

        /// <inheritdoc />
        public override bool Remove(Node neighbour)
        {
            return Contains(neighbour) && Remove(_edges[neighbour.Name]);
        }

        /// <inheritdoc />
        public override bool Remove(Edge item)
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
        public override IEnumerator<Edge> GetEnumerator()
        {
            return _edges.Values.GetEnumerator();
        }
    }
}