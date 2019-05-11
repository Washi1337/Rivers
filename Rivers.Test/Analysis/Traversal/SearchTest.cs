using System.Linq;
using Rivers.Analysis.Traversal;
using Xunit;

namespace Rivers.Test.Analysis.Traversal
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
            var traversal = new BreadthFirstTraversal();
            var orderRecorder = new TraversalOrderRecorder(traversal);
            traversal.Run(Tree.Nodes["1"]);
            
            Assert.True(orderRecorder.GetIndex(Tree.Nodes["2"]) < orderRecorder.GetIndex(Tree.Nodes["4A"]));
            Assert.True(orderRecorder.GetIndex(Tree.Nodes["3A"]) < orderRecorder.GetIndex(Tree.Nodes["4A"]));
        }
        
        [Fact]
        public void DepthFirstOrderTest()
        {
            var traversal = new DepthFirstTraversal();
            var recorder = new TraversalOrderRecorder(traversal);
            traversal.Run(Tree.Nodes["1"]);
            
            if (recorder.GetIndex(Tree.Nodes["2"]) < recorder.GetIndex(Tree.Nodes["3A"]))
                Assert.True(recorder.GetIndex(Tree.Nodes["4A"]) < recorder.GetIndex(Tree.Nodes["3A"]));
            else
                Assert.True(recorder.GetIndex(Tree.Nodes["5"]) < recorder.GetIndex(Tree.Nodes["4A"]));
        }

        [Fact]
        public void PostOrderTest()
        {
            var graph = new Graph();
            graph.Nodes.Add("0");
            graph.Nodes.Add("1");
            graph.Nodes.Add("2");
            graph.Nodes.Add("3");
            graph.Nodes.Add("4");

            graph.Edges.Add("0", "2");
            graph.Edges.Add("2", "1");
            graph.Edges.Add("1", "0");
            graph.Edges.Add("0", "3");
            graph.Edges.Add("3", "4");
            
            var traversal = new DepthFirstTraversal();
            var recorder = new PostOrderRecorder(traversal);
            traversal.Run(graph.Nodes["0"]);

            var order = recorder.GetOrder();
            Assert.Equal(order.Count, graph.Nodes.Count);
            Assert.All(graph.Nodes, n => Assert.True(order.IndexOf(graph.Nodes["0"]) >= order.IndexOf(n)));
            Assert.True(order.IndexOf(graph.Nodes["1"]) < order.IndexOf(graph.Nodes["2"]));
            Assert.True(order.IndexOf(graph.Nodes["4"]) < order.IndexOf(graph.Nodes["3"]));
        }
    }
}