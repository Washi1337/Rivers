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
    }
}