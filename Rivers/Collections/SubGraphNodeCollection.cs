using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers.Collections
{
    public class SubGraphNodeCollection : ICollection<Node>
    {
        private readonly ISet<Node> _nodes = new HashSet<Node>();
        private readonly SubGraph _parent;

        public SubGraphNodeCollection(SubGraph parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public int Count => _nodes.Count;
        
        public bool IsReadOnly => false;

        public void Add(Node item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!ReferenceEquals(item.ParentGraph, _parent.ParentGraph))
                throw new ArgumentException("Node must be present in the same graph.");
            if (_nodes.Add(item))
                item.SubGraphs.Add(_parent);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(Node item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(Node item)
        {
            if (item != null && _nodes.Remove(item))
            {
                item.SubGraphs.Remove(_parent);
                return true;
            }

            return false;
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}