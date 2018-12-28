namespace Rivers.Generators
{
    /// <summary>
    /// Provides a mechanism for generating grid-like graph structures.
    /// </summary>
    public class GridGenerator : IGraphGenerator
    {
        public GridGenerator(bool directed, int width, int height)
        {
            Directed = directed;
            Width = width;
            Height = height;
        }
        
        /// <summary>
        /// Gets a value indicating whether the resulting graph is a directed graph or not.
        /// </summary>
        public bool Directed
        {
            get;
        }
        
        /// <summary>
        /// Gets the amount of nodes that will be present in the horizontal direction of the resulting graph.  
        /// </summary>
        public int Width
        {
            get;
        }

        /// <summary>
        /// Gets the amount of nodes that will be present in the vertical direction of the resulting graph.
        /// </summary>
        public int Height
        {
            get;
        }

        /// <inheritdoc />
        public Graph GenerateGraph()
        {
            var graph = new Graph(Directed);

            // Add nodes.
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                    graph.Nodes.Add($"{x},{y}");
            }

            // Add edges.
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var node = graph.Nodes[$"{x},{y}"];
                    
                    // Top neighbour.
                    if (graph.Nodes.TryGetNode($"{x},{y - 1}", out var topNode))
                        topNode.OutgoingEdges.Add(node);
                    
                    // Left neighbour.
                    if (graph.Nodes.TryGetNode($"{x - 1},{y}", out var leftNode))
                        leftNode.OutgoingEdges.Add(node);
                }
            }

            return graph;
        }
    }
}