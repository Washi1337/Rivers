using Rivers.Collections;

namespace Rivers
{
    /// <summary>
    /// Represents a directed graph.
    /// </summary>
    public class Graph
    {
        public Graph()
        {
            Nodes = new NodeCollection(this);
            Edges = new EdgeCollection(this);
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
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Graph) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}