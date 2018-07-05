using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of edges inside a graph.
    /// </summary>
    public class DirectedEdgeCollection : EdgeCollection
    {
        public DirectedEdgeCollection(Graph parentGraph)
            : base(parentGraph)
        {
        }

        /// <inheritdoc />
        public override int Count => ParentGraph.Nodes.Sum(x => x.IncomingEdges.Count);

        /// <inheritdoc />
        public override Edge Add(Edge edge)
        {
            if (edge == null) 
                throw new ArgumentNullException(nameof(edge));
            if (!ReferenceEquals(edge.ParentGraph, ParentGraph))
                throw new ArgumentException("Edge should be a part of the same graph.");
            
            edge.Source.OutgoingEdges.Add(edge);
            edge.Target.IncomingEdges.Add(edge);
            return edge;
        }

        /// <inheritdoc />
        public override void Clear()
        {
            foreach (var node in ParentGraph.Nodes)
            {
                node.IncomingEdges.Clear();
                node.OutgoingEdges.Clear();
            }
        }

        /// <inheritdoc />
        public override bool Contains(Edge item)
        {
            return item != null
                   && ReferenceEquals(item.ParentGraph, ParentGraph)
                   && item.Source.OutgoingEdges.Contains(item);
        }

        /// <inheritdoc />
        public override void CopyTo(Edge[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var edge in this)
            {
                array[arrayIndex + i] = edge;
                i++;
            }
        }

        /// <inheritdoc />
        public override bool Remove(Edge item)
        {
            if (item == null || !Contains(item))
                return false;

            item.Source.OutgoingEdges.Remove(item);
            item.Target.IncomingEdges.Remove(item);
            return true;
        }

        /// <inheritdoc />
        public override IEnumerator<Edge> GetEnumerator()
        {
            return ParentGraph.Nodes.SelectMany(x => x.IncomingEdges).GetEnumerator();
        }
    }
}