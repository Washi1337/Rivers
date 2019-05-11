using System.Collections.Generic;
using Rivers.Analysis.Connectivity;
using Xunit;

namespace Rivers.Test.Analysis.Connectivity
{
    public class Scc
    {
        [Fact]
        public void Simple()
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

            var components = g.FindStronglyConnectedComponents();

            Assert.Equal(3, components.Count);
            Assert.Contains(new HashSet<Node>
            {
                g.Nodes["0"],
                g.Nodes["1"],
                g.Nodes["2"],
            }, components);
            
            Assert.Contains(new HashSet<Node>
            {
                g.Nodes["3"],
            }, components);
            
            Assert.Contains(new HashSet<Node>
            {
                g.Nodes["4"],
            }, components);
        }
    }
}