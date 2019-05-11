using Xunit;

namespace Rivers.Test
{
    public class GraphTest
    {
        [Fact]
        public void Union()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Edges.Add("1", "2");
            
            var h = new Graph();
            h.Nodes.Add("3");
            h.Nodes.Add("4");
            h.Nodes.Add("5");
            h.Edges.Add("3", "4");
            h.Edges.Add("4", "5");

            g.UnionWith(h);
            
            Assert.Equal(5, g.Nodes.Count);
            Assert.Equal(3, g.Edges.Count);
        }

        [Fact]
        public void Transpose()
        {
            var g = new Graph();
            g.Nodes.Add("0");
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");
            
            g.Edges.Add("0", "2");
            g.Edges.Add("2", "1");
            g.Edges.Add("1", "0");
            g.Edges.Add("0", "3");
            g.Edges.Add("3", "4");

            var transpose = g.Transpose();
            Assert.True(transpose.Nodes["0"].OutgoingEdges.Contains("1"));
            Assert.True(transpose.Nodes["1"].OutgoingEdges.Contains("2"));
            Assert.True(transpose.Nodes["2"].OutgoingEdges.Contains("0"));
            Assert.True(transpose.Nodes["3"].OutgoingEdges.Contains("0"));
            Assert.True(transpose.Nodes["4"].OutgoingEdges.Contains("3"));
        }
    }
}