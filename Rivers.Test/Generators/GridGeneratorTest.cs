using System.IO;
using Rivers.Analysis.Isomorphism;
using Rivers.Generators;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Generators
{
    public class GridGeneratorTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleSquare(bool directed)
        {
            var graphString = @"
strict graph { 
A -- B -- C -- D
E -- F -- G -- H
I -- J -- K -- L
M -- N -- O -- P

A -- E -- I -- M
B -- F -- J -- N
C -- G -- K -- O
D -- H -- L -- P
}";
            if (directed)
            {
                graphString = graphString
                    .Replace("--", "->")
                    .Replace("graph", "digraph");;
            }

            var reader = new StringReader(graphString);
            var g = new DotReader(reader).Read();
            
            var generator = new GridGenerator(directed, 4, 4);
            var h = generator.GenerateGraph();

            var isomorphism = new ColorIsomorphismFinder();
            Assert.True(isomorphism.AreIsomorphic(g, h));
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleRectangle(bool directed)
        {
            string graphString = @"
strict graph { 
A -- B -- C -- D
E -- F -- G -- H
I -- J -- K -- L

A -- E -- I
B -- F -- J
C -- G -- K
D -- H -- L
}";
            if (directed)
            {
                graphString = graphString
                    .Replace("--", "->")
                    .Replace("graph", "digraph");
            }

            var reader = new StringReader(graphString);
            var g = new DotReader(reader).Read();
            
            var generator = new GridGenerator(directed, 4, 3);
            var h = generator.GenerateGraph();

            var isomorphism = new ColorIsomorphismFinder();
            Assert.True(isomorphism.AreIsomorphic(g, h));
        }
        
    }
}