using System;
using System.Collections.Generic;
using System.Linq;
using Rivers.Collections;

namespace Rivers
{
    /// <summary>
    /// Represents a single node in a graph.
    /// </summary>
    public class Node
    {
        public Node(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            
            IncomingEdges = new DirectedAdjacencyCollection(this, false);
            OutgoingEdges = new DirectedAdjacencyCollection(this, true);
            UserData = new Dictionary<object, object>();
        }

        /// <summary>
        /// Gets the parent graph containing the node.
        /// </summary>
        public Graph ParentGraph
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the in degree of the node.
        /// </summary>
        public int InDegree => IncomingEdges.Count;

        /// <summary>
        /// Gets the out degree of the node.
        /// </summary>
        public int OutDegree => OutgoingEdges.Count;

        /// <summary>
        /// Gets a collection of edges that are towards the node.
        /// </summary>
        public AdjacencyCollection IncomingEdges
        {
            get;
        }

        /// <summary>
        /// Gets a collection of edges originating from the node.
        /// </summary>
        public AdjacencyCollection OutgoingEdges
        {
            get;
        }

        /// <summary>
        /// Gets a grouped collection of user data associated to the node.
        /// </summary>
        public IDictionary<object, object> UserData
        {
            get;
        }

        public IEnumerable<Edge> GetEdges()
        {
            return IncomingEdges.Union(OutgoingEdges);
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}