using System;

namespace Rivers.Generators
{
    /// <summary>
    /// Provides a mechanism for generating path graphs. 
    /// </summary>
    public class PathGenerator : IGraphGenerator
    {
        /// <summary>
        /// Creates a new path graph generator.
        /// </summary>
        /// <param name="length">The length of the path generated graphs will have.</param>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when the given length is negative.</exception>
        public PathGenerator(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
            
            Length = length;
        }
        
        /// <summary>
        /// Gets the length of the path that generated graphs will have. 
        /// </summary>
        public int Length
        {
            get;
        }

        /// <inheritdoc />
        public Graph GenerateGraph()
        {
            var g = new Graph();

            for (int i = 1; i <= Length; i++)
            {
                g.Nodes.Add(i.ToString());
                if (i > 1)
                    g.Edges.Add((i - 1).ToString(), i.ToString());
            }
            
            return g;
        }
    }
}