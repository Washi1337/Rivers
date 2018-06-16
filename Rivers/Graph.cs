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
    }
}