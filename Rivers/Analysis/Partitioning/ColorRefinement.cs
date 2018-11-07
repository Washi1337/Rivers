using System;
using System.Collections.Generic;

namespace Rivers.Analysis.Partitioning
{
    /// <summary>
    /// Provides a mechanism for partitioning (coloring) nodes into different categories based on their
    /// similarity in neighbourhood.
    /// </summary>
    public class ColorRefinement
    {
        /// <summary>
        /// Finds a coloring for each node in a graph.
        /// </summary>
        /// <param name="graph">The graph to color in.</param>
        /// <returns>A mapping from node to category (color).</returns>
        public static IDictionary<Node, int> FindColoring(Graph graph)
        {
            var refinement = new ColorRefinement(graph);
            refinement.FindColoring();
            return refinement._colorAssignment;
        }

        /// <summary>
        /// Finds a coloring for each node in a list of graphs.
        /// </summary>
        /// <param name="graphs">The graphs to color in.</param>
        /// <returns>A mapping from node to category (color).</returns>
        public static IDictionary<Node, int> FindColoring(params Graph[] graphs)
        {
            return FindColoring((IList<Graph>) graphs);
        }
        
        /// <summary>
        /// Finds a coloring for each node in a list of graphs.
        /// </summary>
        /// <param name="graphs">The graphs to color in.</param>
        /// <returns>A mapping from node to category (color).</returns>
        public static IDictionary<Node, int> FindColoring(IList<Graph> graphs)
        {
            var union = new Graph(graphs[0].IsDirected);
            for (int i = 0; i < graphs.Count; i++)
                union.DisjointUnionWith(graphs[i], i + "_", false);

            var refinement = new ColorRefinement(union);
            refinement.FindColoring();
            
            var newAssignment = new Dictionary<Node, int>();
            foreach (var entry in refinement._colorAssignment)
            {
                string nodeName = entry.Key.Name;
                
                int index = nodeName.IndexOf('_');
                int graphIndex = int.Parse(nodeName.Remove(index));
                string actualName = nodeName.Substring(index + 1);
                
                newAssignment[graphs[graphIndex].Nodes[actualName]] = entry.Value;
            }
            
            return newAssignment;
        }
        
        private readonly IDictionary<Node, int> _colorAssignment = new Dictionary<Node, int>();
        private readonly IDictionary<int, ISet<Node>> _colorSets = new Dictionary<int, ISet<Node>>();
        private readonly Graph _graph;

        private ColorRefinement(Graph graph)
        {
            _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        }

        /// <summary>
        /// Defines a coarse partitioning that categorizes nodes based on the degree of the node.
        /// </summary>
        private void InitializeColors()
        {
            foreach (var node in _graph.Nodes)
            {
                int color = node.OutDegree;
                _colorAssignment[node] = color;
                
                if (!_colorSets.TryGetValue(color, out var set))
                    _colorSets[color] = set = new HashSet<Node>();
                set.Add(node);
            }
        }

        /// <summary>
        /// Finds the category that has the highest amount of nodes in it.
        /// </summary>
        /// <param name="colorSets">The nodes grouped by their category.</param>
        /// <returns>The category with the most nodes.</returns>
        private static int FindBiggestColorSet(IDictionary<int, ISet<Node>> colorSets)
        {
            int max = -1;
            int key = -1;
            foreach (var entry in colorSets)
            {
                if (max < entry.Value.Count)
                {
                    key = entry.Key;
                    max = entry.Value.Count;
                }
            }

            return key;
        }

        /// <summary>
        /// Determines which categories should be split up next and fills up the queue with it.
        /// </summary>
        /// <param name="queue">The queue to add to.</param>
        /// <param name="colorsInQueue">A set of colors already present in the queue.</param>
        /// <param name="colorSets">The current categories and the associated nodes.</param>
        /// <param name="preservedColor">The color that is preserved, or null if none was preserved.</param>
        private static void FillQueue(Queue<int> queue, ISet<int> colorsInQueue, IDictionary<int, ISet<Node>> colorSets, int? preservedColor)
        {
            int colorToAdd = preservedColor ?? FindBiggestColorSet(colorSets);
            foreach (int color in colorSets.Keys)
            {
                if (color != colorToAdd && colorsInQueue.Add(color))
                    queue.Enqueue(color);
            }
        }

        /// <summary>
        /// Attempts to determine a refinement of the current partitioning. 
        /// </summary>
        /// <param name="color">The color to refine for.</param>
        /// <param name="colorSets">All categories currently determined.</param>
        /// <returns>A refinement of the current partitioning, grouped by color.</returns>
        private Dictionary<int, IDictionary<int, ISet<Node>>> FindRefinements(int color, IDictionary<int, ISet<Node>> colorSets)
        {
            var neighbourCounts = new Dictionary<Node, int>();
            foreach (var node in colorSets[color])
            {
                foreach (var neighbour in node.GetNeighbours())
                {
                    if (_colorAssignment[neighbour] != color)
                    {
                        if (!neighbourCounts.ContainsKey(neighbour))
                            neighbourCounts[neighbour] = 1;
                        else
                            neighbourCounts[neighbour]++;
                    }
                }
            }

            var refinementsByColor = new Dictionary<int, IDictionary<int, ISet<Node>>>();
            foreach (var entry in colorSets)
            {
                var refinements = new Dictionary<int, ISet<Node>>();
                foreach (var node in entry.Value)
                {
                    int count = 0;
                    if (neighbourCounts.ContainsKey(node))
                        count = neighbourCounts[node];
                    if (!refinements.ContainsKey(count))
                        refinements[count] = new HashSet<Node>();
                    refinements[count].Add(node);
                }

                if (refinements.Count > 1)
                    refinementsByColor[color] = refinements;
            }

            return refinementsByColor;
        }

        /// <summary>
        /// Finds a coloring for the graph the class was initialized with.
        /// </summary>
        private void FindColoring()
        {
            var colorsInQueue = new HashSet<int>();
            var queue = new Queue<int>();
            
            InitializeColors();
            int nextColor = _colorSets.Count;
            
            FillQueue(queue, colorsInQueue, _colorSets, null);

            while (queue.Count > 0)
            {
                int currentColor = queue.Dequeue();
                colorsInQueue.Remove(currentColor);

                var newRefinements = FindRefinements(currentColor, _colorSets);
                var addedColors = new Dictionary<int, ISet<Node>>();
                
                foreach (var entry in newRefinements)
                {
                    int otherColor = entry.Key;
                    var refinement = entry.Value;

                    if (refinement.Count > 1)
                    {
                        bool hasUpdatedOriginalCell = false;
                        foreach (var newCell in refinement.Values)
                        {
                            addedColors[otherColor] = newCell;
                            if (hasUpdatedOriginalCell)
                            {
                                foreach (var n in newCell)
                                    _colorAssignment[n] = nextColor;
                                nextColor++;
                            }
                            hasUpdatedOriginalCell = true;
                        }
                        
                        FillQueue(queue, colorsInQueue, addedColors, otherColor);
                    }
                }

                foreach (var entry in addedColors)
                    _colorSets[entry.Key] = entry.Value;
            }
        }
        
    }
}