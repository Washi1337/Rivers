namespace Rivers.Generators
{
    /// <summary>
    /// Provides members for generating graphs.
    /// </summary>
    public interface IGraphGenerator
    {
        /// <summary>
        /// Generates a new graph.
        /// </summary>
        /// <returns>The generated graph.</returns>
        Graph GenerateGraph();
    }
}