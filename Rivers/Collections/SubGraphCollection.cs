using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of sub graphs present in a main graph.
    /// </summary>
    public class SubGraphCollection : ICollection<SubGraph>
    {
        private readonly ISet<SubGraph> _subGraphs = new HashSet<SubGraph>();
        private readonly Graph _parent;

        public SubGraphCollection(Graph parent)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <inheritdoc />
        public int Count => _subGraphs.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(SubGraph item)
        {
            _subGraphs.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var subGraph in _subGraphs)
                Remove(subGraph);
        }

        /// <inheritdoc />
        public bool Contains(SubGraph item)
        {
            return item != null && _subGraphs.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(SubGraph[] array, int arrayIndex)
        {
            _subGraphs.CopyTo(array, arrayIndex);
        }
        
        /// <inheritdoc />
        public bool Remove(SubGraph item)
        {
            if (Contains(item))
            {
                if (item.Nodes.Count > 0)
                {
                    foreach (var node in item.Nodes.ToArray())
                        node.SubGraphs.Remove(item);
                }

                _subGraphs.Remove(item);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerator<SubGraph> GetEnumerator()
        {
            return _subGraphs.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}