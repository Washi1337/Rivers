using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rivers.Analysis.Partitioning;

namespace Rivers.Analysis.Isomorphism
{
    /// <summary>
    /// Provides an implementation of an isomorphism finder using the color refinement algorithm.
    /// </summary>
    public class ColorIsomorphismFinder : IIsomorphismFinder
    {
        private const string GraphAPrefix = "A_";
        private const string GraphBPrefix = "B_";

        /// <inheritdoc />
        public bool AreIsomorphic(Graph graph1, Graph graph2)
        {
            return FindIsomorphisms(graph1, graph2).Any();
        }

        /// <inheritdoc />
        public IEnumerable<IDictionary<Node, Node>> FindIsomorphisms(Graph graph1, Graph graph2)
        {
            // Make a union of the two graphs.
            var union = new Graph(graph1.IsDirected);
            union.DisjointUnionWith(graph1, GraphAPrefix, false);
            union.DisjointUnionWith(graph2, GraphBPrefix, false);

            // Find all isomorphisms.
            foreach (var isomorphism in FindIsomorphisms(union, new List<Node>(), new List<Node>()))
            {            
                var realIsomorphism = new Dictionary<Node, Node>();

                // Transform the found isomorphism to a mapping between the original nodes.
                var nodes = isomorphism.Values.SelectMany(x=>x).ToArray();
                for (int i = 0; i < nodes.Length; i += 2)
                    realIsomorphism[ToRealNode(graph1, graph2, nodes[i])] = ToRealNode(graph1, graph2, nodes[i + 1]);
                
                yield return realIsomorphism;
            }
        }

        private static Node ToRealNode(Graph graph1, Graph graph2, Node n)
        {
            return n.Name.StartsWith(GraphAPrefix)
                ? graph1.Nodes[n.Name.Substring(GraphAPrefix.Length)]
                : graph2.Nodes[n.Name.Substring(GraphBPrefix.Length)];
        }

        private static IEnumerable<IDictionary<int, ISet<Node>>> FindIsomorphisms(Graph g, IList<Node> d, IList<Node> i)
        {
            var refinement = new ColorRefinement(g);
            refinement.D.AddRange(d);
            refinement.I.AddRange(i);
            refinement.RefinePartitioning();
            
            // Unbalanced colorings can never result in isomorphisms.
            if (!IsBalanced(refinement.ColorSets.Values))
                yield break;
            
            // Base case: If we found a bijection, then this is an isomorphism!
            if (IsBijection(refinement.ColorSets.Values))
            {
                yield return refinement.ColorSets;
                yield break;
            }

            // Find any of the cells that can be refined further by choosing one element.
            var bigSet = refinement.ColorSets.Values.First(x => x.Count >= 4);
            var nodeA = bigSet.First(x => x.Name.StartsWith(GraphAPrefix));
            var d1 = new List<Node>(d) {nodeA};
            
            // See if the selected node can be mapped to any of the other graph's nodes.
            foreach (var nodeB in bigSet.Where(x => x.Name.StartsWith(GraphBPrefix)))
            {
                var i1 = new List<Node>(i) {nodeB};
                foreach (var isomorphism in FindIsomorphisms(g, d1, i1))
                    yield return isomorphism;
            }
        }
        
        /// <summary>
        /// Determines whether a partitioning is balanced or not. That is, determines whether each category have the
        /// same size in both graphs.
        /// </summary>
        /// <param name="colorSets">The partitioning.</param>
        /// <returns>True if the partitioning is balanced, false otherwise.</returns>
        private static bool IsBalanced(IEnumerable<ISet<Node>> colorSets)
        {
            foreach (var set in colorSets)
            {
                if (set.Count % 2 != 0)
                    return false;
                
                int countA = set.Count(node => node.Name.StartsWith(GraphAPrefix));
                if (countA != set.Count / 2)
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Determine whether the given partitioning forms a bijection between the two graphs. That is, every node
        /// in one graph maps to exactly one node in the other graph.
        /// </summary>
        /// <param name="colorSets">The partitioning.</param>
        /// <returns>True if the partitioning forms a bijection, false otherwise.</returns>
        private static bool IsBijection(IEnumerable<ISet<Node>> colorSets)
        {
            return colorSets.All(x => x.Count == 2);
        }

    }
}