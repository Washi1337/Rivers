using System.IO;
using System.Linq;
using Rivers.Analysis.Partitioning;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Analysis.Partitioning
{
    public class ColorRefinementTest
    {
        [Fact]
        public void Simple()
        {
            var reader = new DotReader(new StringReader(@"
strict graph {
    1 -- 2 -- 3
}
strict graph {
    3 -- 2 -- 1
}"));
            var g = reader.Read();
            var h = reader.Read();

            var coloring = ColorRefinement.FindColoring(g, h);

            Assert.Equal(6, coloring.Count);
            Assert.Equal(2, coloring.Values.Distinct().Count());
            Assert.Equal(coloring[g.Nodes["1"]], coloring[h.Nodes["1"]]);
            Assert.Equal(coloring[g.Nodes["2"]], coloring[h.Nodes["2"]]);
            Assert.Equal(coloring[g.Nodes["3"]], coloring[h.Nodes["3"]]);
        }
        
        [Fact]
        public void Branching()
        {
            var reader = new DotReader(new StringReader(@"
strict graph {
    1 -- 2 -- 4
    1 -- 3 -- 4
}
strict graph {
    4 -- 2 -- 1
    4 -- 3 -- 1
}"));
            var g = reader.Read();
            var h = reader.Read();

            var coloring = ColorRefinement.FindColoring(g, h);

            Assert.Equal(8, coloring.Count);
            Assert.Single(coloring.Values.Distinct());
            Assert.Equal(coloring[g.Nodes["1"]], coloring[h.Nodes["1"]]);
            Assert.Equal(coloring[g.Nodes["2"]], coloring[h.Nodes["2"]]);
            Assert.Equal(coloring[g.Nodes["3"]], coloring[h.Nodes["3"]]);
            Assert.Equal(coloring[g.Nodes["4"]], coloring[h.Nodes["4"]]);
        }

        [Fact]
        public void Tree()
        {
            var reader = new DotReader(new StringReader(@"
strict graph {
    1 -- 2
    2 -- 3
    2 -- 4
    1 -- 5
}
strict graph {
    1 -- 2
    2 -- 3
    2 -- 4
    1 -- 5
}"));
            var g = reader.Read();
            var h = reader.Read();

            var coloring = ColorRefinement.FindColoring(g, h);

            Assert.Equal(10, coloring.Count);
            Assert.Equal(4, coloring.Values.Distinct().Count());
            Assert.Equal(coloring[g.Nodes["1"]], coloring[h.Nodes["1"]]);
            Assert.Equal(coloring[g.Nodes["2"]], coloring[h.Nodes["2"]]);
            Assert.Equal(coloring[g.Nodes["3"]], coloring[h.Nodes["3"]]);
            Assert.Equal(coloring[g.Nodes["4"]], coloring[h.Nodes["4"]]);
            Assert.Equal(coloring[g.Nodes["5"]], coloring[h.Nodes["5"]]);
        }

    }
}