using System;

namespace Rivers.Generators
{
    /// <summary>
    /// Provides a mechanism for generating complete graphs. That is, each node in the generated graph is connected
    /// to every other node.
    /// </summary>
    public class CompleteGraphGenerator : IGraphGenerator
    {
        /// <summary>
        /// Creates a new complete graph generator.
        /// </summary>
        /// <param name="directed">Determines whether the generator generates directed or undirected graphs.</param>
        /// <param name="size">Determines the amount of nodes to be added to the graphs.</param>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when the size is negative.</exception>
        public CompleteGraphGenerator(bool directed, int size)
        {
            if (Size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be a non-negative number.");

            Directed = directed;
            Size = size;
        }

        /// <summary>
        /// Gets a value indicating whether the generator should generate directed or undirected graphs. 
        /// </summary>
        public bool Directed
        {
            get;
        }

        /// <summary>
        /// Gets the amount of nodes to be added to the graph.
        /// </summary>
        public int Size
        {
            get;
        }

        /// <inheritdoc />
        public Graph GenerateGraph()
        {
            var g = new Graph(Directed);

            for (int i = 1; i <= Size; i++)
                g.Nodes.Add(i.ToString());

            for (int i = 1; i <= Size; i++)
            {
                for (int j = 1; j <= Size; j++)
                {
                    if (i != j)
                        g.Edges.Add(i.ToString(), j.ToString());
                }
            }

            return g;
        }
    }
}