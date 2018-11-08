using System.Collections;
using System.Collections.Generic;

namespace Rivers.Analysis.Isomorphism
{
    /// <summary>
    /// Provides members for finding isomorphisms between two graphs.
    /// </summary>
    public interface IIsomorphismFinder
    {
        /// <summary>
        /// Determines whether two graphs are isomorphic or not.
        /// </summary>
        /// <param name="graph1">The first graph.</param>
        /// <param name="graph2">The second graph.</param>
        /// <returns>True if there exists an isomorphism between the two graphs, false otherwise.</returns>
        bool AreIsomorphic(Graph graph1, Graph graph2);

        /// <summary>
        /// Lazily finds all isomorphisms between two graphs (if any). 
        /// </summary>
        /// <param name="graph1">The first graph.</param>
        /// <param name="graph2">The second graph.</param>
        /// <returns>A collection of bijective dictionaries that map nodes from the first graph to nodes in the second graph.</returns>
        IEnumerable<IDictionary<Node, Node>> FindIsomorphisms(Graph graph1, Graph graph2);
    }
}