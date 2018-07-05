using System;
using System.Linq;
using Rivers.Generators;
using Xunit;

namespace Rivers.Test.Generators
{
    public class PathGeneratorTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void DirectedPathTest(int length)
        {
            var generator = new PathGenerator(true, length);
            var g = generator.GenerateGraph();

            Assert.True(g.IsDirected);
            Assert.Equal(length, g.Nodes.Count);
            Assert.Equal(Math.Max(0, length - 1), g.Edges.Count);
            Assert.All(g.Nodes.Skip(1), n => Assert.Equal(1, n.InDegree));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(100)]
        public void UndirectedPathTest(int length)
        {
            var generator = new PathGenerator(false, length);
            var g = generator.GenerateGraph();

            Assert.False(g.IsDirected);
            Assert.Equal(length, g.Nodes.Count);
            Assert.Equal(Math.Max(0, length - 1), g.Edges.Count);
            Assert.All(g.Nodes.Skip(1).Take(g.Nodes.Count - 2), n => Assert.Equal(2, n.InDegree));
        }
    }
}