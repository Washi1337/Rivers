using Xunit;

namespace Rivers.Test
{
    public class DirectedAdjacencyCollectionTest
    {
        [Fact]
        public void EmptyCollection()
        {
            var g = new Graph(true);
            var node = g.Nodes.Add("1");
            
            Assert.Empty(node.IncomingEdges);
            Assert.Empty(node.OutgoingEdges);
        }

        [Fact]
        public void AddIncomingEdge()
        {
            var g = new Graph(true);
            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = new Edge(node1, node2);
            node2.IncomingEdges.Add(edge);

            Assert.Contains(edge, node1.OutgoingEdges);
            Assert.Single(node1.OutgoingEdges);
            Assert.Contains(edge, node2.IncomingEdges);
            Assert.Single(node2.IncomingEdges);
            Assert.Contains(edge, g.Edges);
            Assert.Single(g.Edges);
        }

        [Fact]
        public void AddOutgoingEdge()
        {
            var g = new Graph(true);
            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = new Edge(node1, node2);
            node1.OutgoingEdges.Add(edge);

            Assert.Contains(edge, node1.OutgoingEdges);
            Assert.Single(node1.OutgoingEdges);
            Assert.Contains(edge, node2.IncomingEdges);
            Assert.Single(node2.IncomingEdges);
            Assert.Contains(edge, g.Edges);
            Assert.Single(g.Edges);
        }
    }
}