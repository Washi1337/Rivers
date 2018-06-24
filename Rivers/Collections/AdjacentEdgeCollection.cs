using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers.Collections
{
    public class AdjacentEdgeCollection : ICollection<Edge>
    {
        private readonly ISet<Edge> _edges = new HashSet<Edge>();

        public AdjacentEdgeCollection(Node origin, bool outgoing)
        {
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            Outgoing = outgoing;
        }

        public bool Outgoing
        {
            get;
        }
        
        public Node Origin
        {
            get;
        }
        
        public int Count
        {
            get { return _edges.Count; }
        }
        
        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(string other)
        {
            Add(Origin.ParentGraph.Nodes[other]);
        }

        public void Add(Node other)
        {
            if (Outgoing)
                Add(new Edge(Origin, other));
            else
                Add(new Edge(other, Origin));
        }

        public void Add(Edge item)
        {
            if (Outgoing && item.Source != Origin)
                throw new ArgumentException("Edge must be originating from the origin.");
            else if (!Outgoing && item.Target != Origin)
                throw new ArgumentException("Edge must have a target equal to the origin.");

            if (_edges.Add(item))
            {
                if (Outgoing)
                    item.Target.IncomingEdges.Add(item);
                else
                    item.Source.OutgoingEdges.Add(item);
            }
        }

        public void Clear()
        {
            foreach (var item in _edges)
            {
                if (Outgoing)
                    item.Target.IncomingEdges.Remove(item);
                else
                    item.Source.OutgoingEdges.Remove(item);
            }
            
            _edges.Clear();
        }

        public bool Contains(Edge item)
        {
            return _edges.Contains(item);
        }

        public void CopyTo(Edge[] array, int arrayIndex)
        {
            _edges.CopyTo(array, arrayIndex);
        }

        public bool Remove(Edge item)
        {
            if (item != null && _edges.Remove(item))
            {
                if (Outgoing)
                    item.Target.IncomingEdges.Remove(item);
                else
                    item.Source.OutgoingEdges.Remove(item);
                return true;
            }

            return false;
        }

        public IEnumerator<Edge> GetEnumerator()
        {
            return _edges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}