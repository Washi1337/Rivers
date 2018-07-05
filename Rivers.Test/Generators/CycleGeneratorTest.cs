using Rivers.Generators;
using Xunit;

namespace Rivers.Test.Generators
{
    public class CycleGeneratorTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        public void DirectedCycleTest(int length)
        {
            var generator = new CycleGenerator(true, length);
            var g = generator.GenerateGraph();

            Assert.True(g.IsDirected);
            Assert.Equal(length, g.Nodes.Count);
            Assert.Equal(length, g.Edges.Count);
            Assert.All(g.Nodes, n => Assert.Equal(1, n.InDegree));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(100)]
        public void UndirectedCycleTest(int length)
        {
            var generator = new CycleGenerator(false, length);
            var g = generator.GenerateGraph();

            Assert.False(g.IsDirected);
            Assert.Equal(length, g.Nodes.Count);
            Assert.Equal(length, g.Edges.Count);
            Assert.All(g.Nodes, n => Assert.Equal(2, n.InDegree));
        }
    }
}