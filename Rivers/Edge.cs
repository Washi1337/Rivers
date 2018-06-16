using System;
using System.Collections.Generic;

namespace Rivers
{
    /// <summary>
    /// Represents a single edge in a graph.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Creates a new edge between two nodes.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The destination node.</param>
        /// <exception cref="ArgumentException">Occurs when trying to connect two nodes from different graphs.</exception>
        public Edge(Node source, Node target)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            
            if (source.ParentGraph != target.ParentGraph)
                throw new ArgumentException("Source and target nodes have to be in the same graph.");
            
            UserData = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the graph containing the edge. 
        /// </summary>
        public Graph ParentGraph
        {
            get { return Source.ParentGraph; }
        }
        
        /// <summary>
        /// Gets the source node.
        /// </summary>
        public Node Source
        {
            get;
        }

        /// <summary>
        /// Gets the destination node.
        /// </summary>
        public Node Target
        {
            get;
        }

        /// <summary>
        /// Provides a grouped collection of user data associated to the edge. 
        /// </summary>
        public IDictionary<object, object> UserData
        {
            get;
        }

        protected bool Equals(Edge other)
        {
            return Equals(Source, other.Source) && Equals(Target, other.Target);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((Edge) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source != null ? Source.GetHashCode() : 0) * 397) ^ (Target != null ? Target.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"({Source}, {Target})";
        }
    }
}