using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Analysis.PathFinding
{
    /// <summary>
    /// Provides an implementation for the A* algorithm for finding paths.
    /// </summary>
    public class AStarPathFinder : IPathFinder
    {
        private readonly Func<Edge, double> _getDistance;
        private readonly Func<Node, Node, double> _heuristic;

        /// <summary>
        /// Creates a new instance of the A* path finder.
        /// </summary>
        /// <param name="distancePropertyName">The name of the attribute in <see cref="Edge.UserData"/> to use as the distance.</param>
        /// <param name="heuristic">The heuristic function to use as a distance estimate between two nodes.</param>
        public AStarPathFinder(string distancePropertyName, Func<Node, Node, double> heuristic)
            : this(e => Convert.ToDouble(e.UserData[distancePropertyName]), heuristic)
        {
        }
        
        /// <summary>
        /// Creates a new instance of the A* path finder.
        /// </summary>
        /// <param name="getDistance">A function that extracts the distance from an edge.</param>
        /// <param name="heuristic">The heuristic function to use as a distance estimate between two nodes.</param>
        public AStarPathFinder(Func<Edge, double> getDistance, Func<Node, Node, double> heuristic)
        {
            _getDistance = getDistance ?? throw new ArgumentNullException(nameof(getDistance));
            _heuristic = heuristic ?? throw new ArgumentNullException(nameof(heuristic));
        }

        /// <inheritdoc />
        public IList<Node> FindPath(Node source, Node destination)
        {
            var table = new DistanceTable(source);
            
            var closedSet = new HashSet<Node>();
            var openSet = new HashSet<Node> {source};

            var heuristics = new Dictionary<Node, double> {[source] = _heuristic(source, destination)};

            while (openSet.Count > 0)
            {
                var current = openSet.Aggregate((min, n) => heuristics[n] < heuristics[min] ? n : min);
                if (current == destination)
                    return table.GetShortestPath(destination);

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var edge in current.OutgoingEdges)
                {
                    var neighbour = edge.GetOtherNode(current);
                    if (closedSet.Contains(neighbour))
                        continue;

                    double newDistance = table.Distances.GetOrDefault(current, double.PositiveInfinity) + _getDistance(edge);

                    if (!openSet.Add(neighbour)
                        && newDistance >= table.Distances.GetOrDefault(neighbour, double.PositiveInfinity))
                    {
                        continue;
                    }

                    table.Previous[neighbour] = current;
                    table.Distances[neighbour] = newDistance;
                    heuristics[neighbour] = newDistance + _heuristic(neighbour, destination);
                }
            }

            return table.GetShortestPath(destination);
        }
    }
}