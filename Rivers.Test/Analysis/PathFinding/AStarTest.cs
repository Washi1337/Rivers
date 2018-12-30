using System;
using System.IO;
using System.Linq;
using Rivers.Analysis.PathFinding;
using Rivers.Generators;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Analysis.PathFinding
{
    public class AStarTest
    {
        private readonly struct Point
        {
            public readonly int X;
            public readonly int Y;

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public double GetDistanceTo(Point other)
            {
                int dx = X - other.X;
                int dy = Y - other.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }
        }
        
        private const string DistanceProperty = "distance";
        private const string LocationProperty = "location";
        
        [Fact]
        public void Simple()
        {
            var grid = new GridGenerator(false, 3, 2).GenerateGraph();

            foreach (var node in grid.Nodes)
            {
                var coordinates = node.Name.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);
                node.UserData[LocationProperty] = new Point(x, y);
            }
            
            foreach (var edge in grid.Edges)
                edge.UserData[DistanceProperty] = 1;

            grid.Edges["0,0", "1,0"].UserData[DistanceProperty]
                = grid.Edges["1,0", "2,0"].UserData[DistanceProperty]
                = 3;

            // Perfect heuristic should result in the best path.
            var finder = new AStarPathFinder(DistanceProperty, DefaultHeuristic);
            var path = finder.FindPath(grid.Nodes["0,0"], grid.Nodes["2,0"]).ToArray();
            Assert.Equal(new[]
            {
                grid.Nodes["0,0"],
                grid.Nodes["0,1"],
                grid.Nodes["1,1"],
                grid.Nodes["2,1"],
                grid.Nodes["2,0"]
            }, path);
            
            // Over estimating the heuristic should result in the more costly path.
            var finder2 = new AStarPathFinder(DistanceProperty, (node, node1) => 5*DefaultHeuristic(node, node1));
            var path2 = finder2.FindPath(grid.Nodes["0,0"], grid.Nodes["2,0"]).ToArray();
            Assert.Equal(new[]
            {
                grid.Nodes["0,0"],
                grid.Nodes["1,0"],
                grid.Nodes["2,0"]
            }, path2);
        }

        private static double DefaultHeuristic(Node x, Node y)
        {
            return ((Point) x.UserData[LocationProperty]).GetDistanceTo((Point) y.UserData[LocationProperty]);
        }
    }
}