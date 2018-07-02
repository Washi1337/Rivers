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
        public void PathTest(int length)
        {
            var generator = new CycleGenerator(length);
            var g = generator.GenerateGraph();

            Assert.Equal(length, g.Nodes.Count);
            Assert.Equal(length, g.Edges.Count);
            Assert.All(g.Nodes, n => Assert.Equal(1, n.InDegree));
        }
    }
}