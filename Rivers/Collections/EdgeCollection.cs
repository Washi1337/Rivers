using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Collections
{
    /// <summary>
    /// Represents a collection of edges inside a graph.
    /// </summary>
    public class EdgeCollection : ICollection<Edge>
    {
        public EdgeCollection(Graph parentGraph)
        {
            ParentGraph = parentGraph ?? throw new ArgumentNullException(nameof(parentGraph));
        }
        
        /// <summary>
        /// Gets the graph that contains the collection of edges.
        /// </summary>
        public Graph ParentGraph
        {
            get;
        }

        /// <inheritdoc />
        public int Count
        {
            get { return ParentGraph.Nodes.Sum(x => x.IncomingEdges.Count); }
        }

        /// <inheritdoc />
        public bool IsReadOnly => false;

        public bool TryGetEdge(string source, string target, out Edge edge)
        {
            if (ParentGraph.Nodes.TryGetNode(source, out var sourceNode)
                && ParentGraph.Nodes.TryGetNode(target, out var targetNode))
            {
                if (sourceNode.OutgoingEdges.TryGetEdge(target, out edge))
                    return true;
            }
            
            edge = null;
            return false;
        }

        public Edge Add(string source, string target)
        {
            return Add(ParentGraph.Nodes[source], ParentGraph.Nodes[target]);
        }
        
        /// <summary>
        /// Adds an edge to the graph between two nodes.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>The edge that was created.</returns>
        public Edge Add(Node source, Node target)
        {
            var edge = new Edge(source, target);
            Add(edge);
            return edge;
        }

        /// <inheritdoc />
        public void Add(Edge item)
        {
            if (item == null) 
                throw new ArgumentNullException(nameof(item));
            if (item.ParentGraph != ParentGraph)
                throw new ArgumentException("Edge should be a part of the same graph.");
            
            item.Source.OutgoingEdges.Add(item);
            item.Target.IncomingEdges.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            foreach (var node in ParentGraph.Nodes)
            {
                node.IncomingEdges.Clear();
                node.OutgoingEdges.Clear();
            }
        }

        /// <inheritdoc />
        public bool Contains(Edge item)
        {
            if (item == null)
                return false;

            return item.ParentGraph == ParentGraph && item.Source.OutgoingEdges.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(Edge[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var edge in this)
            {
                array[arrayIndex + i] = edge;
                i++;
            }
        }

        /// <inheritdoc />
        public bool Remove(Edge item)
        {
            if (item == null || !Contains(item))
                return false;

            item.Source.OutgoingEdges.Remove(item);
            item.Target.IncomingEdges.Remove(item);
            return true;
        }

        /// <inheritdoc />
        public IEnumerator<Edge> GetEnumerator()
        {
            foreach (var node in ParentGraph.Nodes)
            {
                foreach (var edge in node.IncomingEdges)
                    yield return edge;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}