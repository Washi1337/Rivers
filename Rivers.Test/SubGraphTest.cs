using System;
using Xunit;

namespace Rivers.Test
{
    public class SubGraphTest
    {
        [Fact]
        public void SimpleSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");

            var subGraph = new SubGraph(g.Nodes["1"], g.Nodes["2"]);
            g.SubGraphs.Add(subGraph);

            Assert.Single(g.Nodes["1"].SubGraphs, subGraph);
            Assert.Single(g.Nodes["2"].SubGraphs, subGraph);
            Assert.Empty(g.Nodes["3"].SubGraphs);
        }

        [Fact]
        public void AddNodeToSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");

            var subGraph = new SubGraph(g.Nodes["1"], g.Nodes["2"]);
            g.SubGraphs.Add(subGraph);
            
            Assert.Empty(g.Nodes["3"].SubGraphs);
            subGraph.Nodes.Add(g.Nodes["3"]);
            Assert.Single(g.Nodes["3"].SubGraphs, subGraph);
        }

        [Fact]
        public void AddNodeFromOtherGraph()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");    
            
            var h = new Graph();
            h.Nodes.Add("4");

            Assert.Throws<ArgumentException>(() => new SubGraph(g.Nodes["1"], h.Nodes["4"]));
            Assert.Throws<ArgumentException>(() =>
            {
                var subGraph = new SubGraph(g.Nodes["1"]);
                subGraph.Nodes.Add(h.Nodes["4"]);
            });
        }

        [Fact]
        public void RemoveNodeFromGraph()
        {
            var g = new Graph();
            var n1 = g.Nodes.Add("1");
            var n2 = g.Nodes.Add("2");
            g.Nodes.Add("3");

            var subGraph = new SubGraph(n1, n2);
            g.SubGraphs.Add(subGraph);
            
            g.Nodes.Remove(n1);
            Assert.Empty(n1.SubGraphs);
            Assert.Single(subGraph.Nodes, n2);
        }

        [Fact]
        public void RemoveEntireSubGraph()
        {
            var g = new Graph();
            var n1 = g.Nodes.Add("1");
            var n2 = g.Nodes.Add("2");
            g.Nodes.Add("3");

            var subGraph = new SubGraph(n1, n2);
            g.SubGraphs.Add(subGraph);

            Assert.Single(n1.SubGraphs, subGraph);
            Assert.Single(n2.SubGraphs, subGraph);
            
            g.SubGraphs.Remove(subGraph);
            Assert.Empty(n1.SubGraphs);
            Assert.Empty(n2.SubGraphs);

        }
    }
}