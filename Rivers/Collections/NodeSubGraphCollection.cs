using System;
using System.Collections;
using System.Collections.Generic;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of sub graphs a particular node is associated with.
    /// </summary>
    public class NodeSubGraphCollection : ICollection<SubGraph>
    {
        private readonly ISet<SubGraph> _items = new HashSet<SubGraph>();
        private readonly Node _parent;

        public NodeSubGraphCollection(Node parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public int Count => _items.Count;

        public bool IsReadOnly => _items.IsReadOnly;
        
        public void Add(SubGraph item)
        {
            if (_items.Add(item))
                item.Nodes.Add(_parent);
        }

        public void Clear()
        {
            foreach (var item in _items)
                Remove(item);
        }

        public bool Contains(SubGraph item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(SubGraph[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(SubGraph item)
        {
            if (_items.Remove(item))
            {
                item.Nodes.Remove(_parent);
                return true;
            }

            return false;
        }
        
        public IEnumerator<SubGraph> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _items).GetEnumerator();
        }
    }
}