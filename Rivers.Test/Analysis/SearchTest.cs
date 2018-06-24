using System.Linq;
using Rivers.Analysis;
using Xunit;

namespace Rivers.Test.Analysis
{
    public class SearchTest
    {
        private static readonly Graph Tree;

        static SearchTest()
        {
            Tree = new Graph();
            Tree.Nodes.Add("1");
            Tree.Nodes.Add("2");
            Tree.Nodes.Add("3A");
            Tree.Nodes.Add("4A");
            Tree.Nodes.Add("5");
            Tree.Nodes.Add("6");

            Tree.Edges.Add("1", "2");
            Tree.Edges.Add("1", "3A");
            Tree.Edges.Add("2", "4A");
            Tree.Edges.Add("2", "5");
            Tree.Edges.Add("3A", "6");
        }

        [Fact]
        public void BreadthFirstTest()
        {
            Assert.Contains(Tree.Nodes["1"].BreadthFirstSearch(n => n.OutgoingEdges.Count == 0), new []
            {
                Tree.Nodes["4A"],
                Tree.Nodes["5"],
                Tree.Nodes["6"],
            });
        }

        [Fact]
        public void DepthFirstTest()
        {
            Assert.Contains(Tree.Nodes["1"].DepthFirstSearch(n => n.OutgoingEdges.Count == 0), new []
            {
                Tree.Nodes["4A"],
                Tree.Nodes["5"],
                Tree.Nodes["6"],
            });
        }

        [Fact]
        public void BreadthFirstOrderTest()
        {
            var order = Tree.Nodes["1"].BreadthFirstTraversal().ToList();
            Assert.True(order.IndexOf(Tree.Nodes["2"]) < order.IndexOf(Tree.Nodes["4A"]));
            Assert.True(order.IndexOf(Tree.Nodes["3A"]) < order.IndexOf(Tree.Nodes["4A"]));
        }
        
        [Fact]
        public void DepthFirstOrderTest()
        {
            var order = Tree.Nodes["1"].DepthFirstTraversal().ToList();
            if (order.IndexOf(Tree.Nodes["2"]) < order.IndexOf(Tree.Nodes["3A"]))
                Assert.True(order.IndexOf(Tree.Nodes["4A"]) < order.IndexOf(Tree.Nodes["3A"]));
            else
                Assert.True(order.IndexOf(Tree.Nodes["5"]) < order.IndexOf(Tree.Nodes["4A"]));
        }

        [Fact]
        public void CyclicTest()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");
            g.Nodes.Add("5");

            g.Edges.Add("1", "2");
            g.Edges.Add("2", "3");
            g.Edges.Add("3", "1");
            
            g.Edges.Add("4", "5");
            
            Assert.True(g.IsCyclic());
        }

        [Fact]
        public void AcyclicTest()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");
            g.Nodes.Add("5");

            g.Edges.Add("1", "2");
            g.Edges.Add("2", "3");
            
            g.Edges.Add("4", "5");
            
            Assert.False(g.IsCyclic());
        }
    }
}