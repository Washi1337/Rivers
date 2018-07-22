using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis.PathFinding
{
    /// <summary>
    /// Provides an implementation for Dijkstra's Algorithm for finding paths.
    /// </summary>
    public class DijkstraPathFinder : IPathFinder
    {
        private readonly Func<Edge, double> _getDistance;

        /// <summary>
        /// Creates a new instance of the Dijkstra's path finder.
        /// </summary>
        /// <param name="distanceProperptyName">The name of the attribute in <see cref="Edge.UserData"/> to use as the distance.</param>
        public DijkstraPathFinder(string distanceProperptyName)
            : this(e => Convert.ToDouble(e.UserData[distanceProperptyName])) 
        {
        }

        /// <summary>
        /// Creates a new instance of the Dijkstra's path finder.
        /// </summary>
        /// <param name="getDistance">A function that extracts the distance from an edge.</param>
        public DijkstraPathFinder(Func<Edge, double> getDistance)
        {
            _getDistance = getDistance ?? throw new ArgumentNullException(nameof(getDistance));
        }

        /// <inheritdoc />
        public IList<Node> FindPath(Node source, Node destination)
        {
            var table = GetDistanceTable(source);
            return table.GetShortestPath(destination);
        }
        
        /// <summary>
        /// Computes a distance table for the given node.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <returns>The computed distance table.</returns>
        public DistanceTable GetDistanceTable(Node source)
        {
            var agenda = new List<Node>();
            var table = new DistanceTable(source);
            
            foreach (var v in source.ParentGraph.Nodes)
            {
                table.Distances[v] = double.PositiveInfinity;
                table.Previous[v] = null;
                agenda.Add(v);
            }

            table.Distances[source] = 0;

            while (agenda.Count > 0)
            {
                var current = agenda.Aggregate((min, n) => table.Distances[n] < table.Distances[min] ? n : min);
                agenda.Remove(current);

                foreach (var edge in current.OutgoingEdges)
                {
                    double newDistance = table.Distances[current] + _getDistance(edge);
                    var next = edge.GetOtherNode(current);
                    if (newDistance < table.Distances[next])
                    {
                        table.Distances[next] = newDistance;
                        table.Previous[next] = current;
                    }
                }
            }

            return table;
        }
    }
}