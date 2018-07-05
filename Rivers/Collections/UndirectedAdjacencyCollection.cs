using System;
using System.Collections.Generic;

namespace Rivers.Collections
{
    public class UndirectedAdjacencyCollection : AdjacencyCollection
    {
        private readonly IDictionary<string, Edge> _edges = new Dictionary<string, Edge>();
        
        public UndirectedAdjacencyCollection(Node origin)
            : base(origin)
        {
        }

        public override int Count => _edges.Count;

        public override Edge this[string neighbourName] => _edges[neighbourName];

        public override Edge this[Node neighbour]
        {
            get 
            { 
                var edge = _edges[neighbour.Name];
                if (ReferenceEquals(edge.Source, neighbour) || ReferenceEquals(edge.Target, neighbour))
                    return edge;
                throw new KeyNotFoundException();
            }
        }

        public override bool TryGetEdge(string neighbourName, out Edge edge)
        {
            return _edges.TryGetValue(neighbourName, out edge);
        }

        public override bool TryGetEdge(Node neighbour, out Edge edge)
        {
            return _edges.TryGetValue(neighbour.Name, out edge) &&
                   (ReferenceEquals(edge.Source, neighbour) || ReferenceEquals(edge.Target, neighbour));
        }
        
        public override void Add(Node neighbour)
        {
            if (neighbour == null)
                throw new ArgumentNullException(nameof(neighbour));
            
            Add(new Edge(Origin, neighbour));
        }

        public override void Add(Edge edge)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            
            if (!ReferenceEquals(edge.Source, Origin) && !ReferenceEquals(edge.Target, Origin))
                throw new ArgumentException("Edge must be either be originating from-, or targeting the origin.");

            var neighbour = ReferenceEquals(edge.Source, Origin) ? edge.Target : edge.Source;

            if (!_edges.ContainsKey(neighbour.Name))
            {
                _edges.Add(neighbour.Name, edge);
                neighbour.IncomingEdges.Add(edge);
            }
        }

        public override void Clear()
        {
            foreach (var item in _edges.Values)
            {
                item.Target.IncomingEdges.Remove(item);
            }
            
            _edges.Clear();
        }
        
        public override bool Contains(Edge edge)
        {
            return edge != null
                   && (TryGetEdge(edge.Source, out var e) || TryGetEdge(edge.Target, out e))
                   && ReferenceEquals(e, edge);
        }

        public override void CopyTo(Edge[] array, int arrayIndex)
        {
            _edges.Values.CopyTo(array, arrayIndex);
        }

        public override bool Remove(Node neighbour)
        {
            return Contains(neighbour) && Remove(_edges[neighbour.Name]);
        }

        public override bool Remove(Edge edge)
        {
            if (edge == null)
                return false;

            var neighbour = ReferenceEquals(Origin, edge.Source) ? edge.Target : edge.Source; 
            if (_edges.ContainsKey(neighbour.Name))
            {
                _edges.Remove(neighbour.Name);
                edge.Target.IncomingEdges.Remove(edge);
                return true;
            }

            return false;
        }

        public override IEnumerator<Edge> GetEnumerator()
        {
            return _edges.Values.GetEnumerator();
        }
    }
}