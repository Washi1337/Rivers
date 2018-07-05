using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents an empty adjacency collection of a node that has not been added to a graph yet.
    /// </summary>
    public class EmptyAdjacencyCollection : AdjacencyCollection
    {
        public EmptyAdjacencyCollection(Node origin)
            : base(origin)
        {
        }

        /// <inheritdoc />
        public override int Count => 0;

        /// <inheritdoc />
        public override Edge this[string neighbour] => throw new KeyNotFoundException();

        /// <inheritdoc />
        public override Edge this[Node neighbour] => throw new KeyNotFoundException();

        /// <inheritdoc />
        public override bool TryGetEdge(string neighbourName, out Edge edge)
        {
            edge = null;
            return false;
        }

        /// <inheritdoc />
        public override bool TryGetEdge(Node neighbour, out Edge edge)
        {
            edge = null;
            return false;
        }

        /// <inheritdoc />
        public override void Add(string neighbourName)
        {
            throw new InvalidOperationException("Cannot add edges to a node that is not added to a graph yet.");
        }

        /// <inheritdoc />
        public override void Add(Node neighbour)
        {
            throw new InvalidOperationException("Cannot add edges to a node that is not added to a graph yet.");
        }

        /// <inheritdoc />
        public override void Add(Edge edge)
        {
            throw new InvalidOperationException("Cannot add edges to a node that is not added to a graph yet.");
        }

        /// <inheritdoc />
        public override void Clear()
        {
        }

        /// <inheritdoc />
        public override bool Contains(Edge edge)
        {
            return false;
        }

        /// <inheritdoc />
        public override void CopyTo(Edge[] array, int arrayIndex)
        {
        }

        /// <inheritdoc />
        public override bool Remove(string neighbourName)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Remove(Node neighbour)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Remove(Edge item)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Contains(string neighbourName)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Contains(Node neighbour)
        {
            return false;
        }

        /// <inheritdoc />
        public override IEnumerator<Edge> GetEnumerator()
        {
            return Enumerable.Empty<Edge>().GetEnumerator();
        }
    }
}