using System.IO;
using System.Linq;
using Rivers.Analysis.PathFinding;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Analysis.PathFinding
{
    public class DijkstraTest
    {
        private const string DistanceProperty = "distance";

        [Fact]
        public void SameNode()
        {
            var reader = new StringReader(
                @"graph {
A -- B -- C -- A
}");
            var dotReader = new DotReader(reader);
            var g = dotReader.Read();
            g.Edges["A", "B"].UserData[DistanceProperty] = 1;
            g.Edges["B", "C"].UserData[DistanceProperty] = 2;
            g.Edges["C", "A"].UserData[DistanceProperty] = 2;
            
            var finder = new DijkstraPathFinder(DistanceProperty);
            var path = finder.FindPath(g.Nodes["A"], g.Nodes["A"]).ToArray();
            Assert.Equal(new[] {g.Nodes["A"]}, path);
        }
        
        [Fact]
        public void SinglePath()
        {
            var reader = new StringReader(
                @"graph {
A -- B -- C
}");
            var dotReader = new DotReader(reader);
            var g = dotReader.Read();
            g.Edges["A", "B"].UserData[DistanceProperty] = 1;
            g.Edges["B", "C"].UserData[DistanceProperty] = 2;
            
            var finder = new DijkstraPathFinder(DistanceProperty);
            var path = finder.FindPath(g.Nodes["A"], g.Nodes["C"]).ToArray();
            Assert.Equal(new[] {g.Nodes["A"], g.Nodes["B"], g.Nodes["C"]}, path);
        }
        
        [Fact]
        public void AlternativePath()
        {
            var reader = new StringReader(
                @"graph {
A -- B -- C -- D
A -- D
}");
            var dotReader = new DotReader(reader);
            var g = dotReader.Read();
            g.Edges["A", "B"].UserData[DistanceProperty] = 1;
            g.Edges["B", "C"].UserData[DistanceProperty] = 2;
            g.Edges["C", "D"].UserData[DistanceProperty] = 3;
            g.Edges["A", "D"].UserData[DistanceProperty] = 100;
            
            var finder = new DijkstraPathFinder(DistanceProperty);
            var path = finder.FindPath(g.Nodes["A"], g.Nodes["D"]).ToArray();
            Assert.Equal(new[] {g.Nodes["A"], g.Nodes["B"], g.Nodes["C"], g.Nodes["D"]}, path);
        }
        
        [Fact]
        public void UnreachableNode()
        {
            var reader = new StringReader(
                @"graph {
A -- B -- C -- D
E
}");
            var dotReader = new DotReader(reader);
            var g = dotReader.Read();
            g.Edges["A", "B"].UserData[DistanceProperty] = 1;
            g.Edges["B", "C"].UserData[DistanceProperty] = 2;
            g.Edges["C", "D"].UserData[DistanceProperty] = 3;
            
            var finder = new DijkstraPathFinder(DistanceProperty);
            Assert.Null(finder.FindPath(g.Nodes["A"], g.Nodes["E"]));
        }
    }
}