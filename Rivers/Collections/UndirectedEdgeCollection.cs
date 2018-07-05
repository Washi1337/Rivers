using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    public class UndirectedEdgeCollection : EdgeCollection
    {
        public UndirectedEdgeCollection(Graph parentGraph) 
            : base(parentGraph)
        {
        }

        /// <inheritdoc />
        public override int Count
        {
            get
            {
                int edges = 0;
                int selfLoops = 0;
                foreach (var edge in ParentGraph.Nodes.SelectMany(x => x.OutgoingEdges))
                {
                    if (edge.Source == edge.Target)
                        selfLoops++;
                    else
                        edges++;
                }

                return edges / 2 + selfLoops;
            }
        }

        public override Edge Add(Edge edge)
        {
            if (edge == null) 
                throw new ArgumentNullException(nameof(edge));
            if (!ReferenceEquals(edge.ParentGraph, ParentGraph))
                throw new ArgumentException("Edge should be a part of the same graph.");

            if (!edge.Source.OutgoingEdges.TryGetEdge(edge.Target, out var e))
            {
                edge.Source.OutgoingEdges.Add(edge);
                return edge;
            }

            return e;
        }

        public override void Clear()
        {
            foreach (var node in ParentGraph.Nodes)
                node.OutgoingEdges.Clear();
        }

        public override bool Contains(Edge item)
        {
            return item != null
                   && ReferenceEquals(item.ParentGraph, ParentGraph)
                   && item.Source.OutgoingEdges.Contains(item);
        }

        public override void CopyTo(Edge[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var edge in this)
            {
                array[arrayIndex + i] = edge;
                i++;
            }
        }

        public override bool Remove(Edge item)
        {
            if (item == null || !Contains(item))
                return false;

            item.Source.OutgoingEdges.Remove(item);
            return true;
        }

        public override IEnumerator<Edge> GetEnumerator()
        {
            return ParentGraph.Nodes.SelectMany(x => x.IncomingEdges).Distinct().GetEnumerator();
        }
    }
}