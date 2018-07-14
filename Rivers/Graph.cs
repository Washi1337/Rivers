using System;
using System.Linq;
using System.Reflection.Emit;
using Rivers.Collections;

namespace Rivers
{
    /// <summary>
    /// Represents a directed graph.
    /// </summary>
    public class Graph
    {
        public Graph()
            : this(true)
        {
        }
        
        public Graph(bool isDirected)
        {
            IsDirected = isDirected;
            Nodes = new NodeCollection(this);
            
            if (isDirected)
                Edges = new DirectedEdgeCollection(this);
            else
                Edges = new UndirectedEdgeCollection(this);
        }

        public bool IsDirected
        {
            get;
        }

        /// <summary>
        /// Gets a collection of nodes present in the graph.
        /// </summary>
        public NodeCollection Nodes
        {
            get;
        }

        /// <summary>
        /// Gets a collection of edges present in the graph.
        /// </summary>
        public EdgeCollection Edges
        {
            get;
        }

        protected bool Equals(Graph other)
        {
            var comparer = new GraphComparer();
            return comparer.Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            
            return Equals((Graph) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }

        public bool IsDisjointWith(Graph other)
        {
            return Nodes.Count >= other.Nodes.Count 
                ? Nodes.Any(x => other.Nodes.Contains(x)) 
                : other.Nodes.Any(x => Nodes.Contains(x));
        }

        public void UnionWith(Graph other, bool includeUserData=true)
        {
            foreach (var otherNode in other.Nodes)
            {
                var node = new Node(otherNode.Name);
                if (includeUserData)
                {
                    foreach (var entry in otherNode.UserData)
                        node.UserData[entry.Key] = entry.Value;
                }
                Nodes.Add(node);
            }

            foreach (var otherEdge in other.Edges)
            {
                var edge = new Edge(Nodes[otherEdge.Source.Name], Nodes[otherEdge.Target.Name]);
                if (includeUserData)
                {
                    foreach (var entry in otherEdge.UserData)
                        edge.UserData[entry.Key] = entry.Value;
                }
                Edges.Add(edge);
            }
        }

        public void DisjointUnionWith(Graph other, string nodePrefix, bool includeUserData=true)
        {
            foreach (var otherNode in other.Nodes)
            {
                var node = new Node(nodePrefix + otherNode.Name);
                if (includeUserData)
                {
                    foreach (var entry in otherNode.UserData)
                        node.UserData[entry.Key] = entry.Value;
                }
                Nodes.Add(node);
            }

            foreach (var otherEdge in other.Edges)
            {
                var edge = new Edge(Nodes[nodePrefix + otherEdge.Source.Name], Nodes[nodePrefix + otherEdge.Target.Name]);
                if (includeUserData)
                {
                    foreach (var entry in otherEdge.UserData)
                        edge.UserData[entry.Key] = entry.Value;
                }
                Edges.Add(edge);
            }
        }

        public Graph ToUndirected()
        {
            var g = new Graph(false);
            
            foreach (var node in Nodes)
                g.Nodes.Add(node.Name);
            foreach (var edge in Edges)
                g.Edges.Add(edge.Source.Name, edge.Target.Name);

            return g;
        }
        
    }
}