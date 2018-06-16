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
            
            IncomingEdges = new AdjacentEdgeCollection(this, false);
            OutgoingEdges = new AdjacentEdgeCollection(this, true);
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
        /// Gets a collection of edges that are towards the node.
        /// </summary>
        public AdjacentEdgeCollection IncomingEdges
        {
            get;
        }

        /// <summary>
        /// Gets a collection of edges originating from the node.
        /// </summary>
        public AdjacentEdgeCollection OutgoingEdges
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

        public IEnumerable<Node> GetPredecessors()
        {
            return IncomingEdges.Select(x => x.Source);
        }

        public IEnumerable<Node> GetSuccessors()
        {
            return OutgoingEdges.Select(x => x.Target);
        }
        
        /// <summary>
        /// Gets a collection of neighbouring nodes.
        /// </summary>
        public IEnumerable<Node> GetNeighbours()
        {
            return GetPredecessors().Union(GetSuccessors());
        }

        public IEnumerable<Node> PreOrderTraversal()
        {       
            yield return this;
            
            foreach (var successor in GetSuccessors())
            {
                foreach (var node in successor.PreOrderTraversal())
                    yield return node;
            }
        }

        public IEnumerable<Node> PostOrderTraversal()
        {
            var visited = new HashSet<Node>();
            
            foreach (var successor in GetSuccessors())
            {
                foreach (var node in successor.PostOrderTraversal())
                    yield return node;
            }

            yield return this;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}