using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rivers.Analysis.Isomorphism;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Analysis.Isomorphism
{
    public class IsomorphismTest
    {
        [Fact]
        public void CoarsestNotABijection()
        {
            var reader = new DotReader(new StringReader(@"
            strict graph {
                1 -- 2 -- 3 -- 4 -- 5 -- 6 -- 7
                3 -- 5
            }
            strict graph {
                1 -- 2 -- 3 -- 4 -- 5 -- 6 -- 7
                3 -- 5
            }"));
            var g = reader.Read();
            var h = reader.Read();

            var isomorphismFinder = new ColorIsomorphismFinder();

            var isomorphisms = isomorphismFinder.FindIsomorphisms(g, h).ToArray();
            Assert.Equal(2, isomorphisms.Length);

            var correctIsomorphisms = new[]
            {
                new Dictionary<Node, Node>
                {
                    [g.Nodes["1"]] = h.Nodes["1"],
                    [g.Nodes["2"]] = h.Nodes["2"],
                    [g.Nodes["3"]] = h.Nodes["3"],
                    [g.Nodes["4"]] = h.Nodes["4"],
                    [g.Nodes["5"]] = h.Nodes["5"],
                    [g.Nodes["6"]] = h.Nodes["6"],
                    [g.Nodes["7"]] = h.Nodes["7"],
                },
                new Dictionary<Node, Node>
                {
                    [g.Nodes["1"]] = h.Nodes["7"],
                    [g.Nodes["2"]] = h.Nodes["6"],
                    [g.Nodes["3"]] = h.Nodes["5"],
                    [g.Nodes["4"]] = h.Nodes["4"],
                    [g.Nodes["5"]] = h.Nodes["3"],
                    [g.Nodes["6"]] = h.Nodes["2"],
                    [g.Nodes["7"]] = h.Nodes["1"],
                },
            };

            Assert.All(isomorphisms, x => Assert.Contains(x, correctIsomorphisms));
        }
        
    }
}