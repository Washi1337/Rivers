using Xunit;

namespace Rivers.Test
{
    public class UndirectedAdjacencyCollectionTest
    {
        [Fact]
        public void EmptyCollection()
        {
            var g = new Graph(false);
            var node = g.Nodes.Add("1");
            
            Assert.Equal(0, node.IncomingEdges.Count);
            Assert.Equal(0, node.OutgoingEdges.Count);
        }

        [Fact]
        public void AddIncomingEdge()
        {
            var g = new Graph(false);
            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = new Edge(node1, node2);
            node2.IncomingEdges.Add(edge);

            Assert.Contains(edge, node1.IncomingEdges);
            Assert.Equal(1, node1.OutgoingEdges.Count);
            Assert.Contains(edge, node1.OutgoingEdges);
            Assert.Equal(1, node1.OutgoingEdges.Count);
            
            Assert.Contains(edge, node2.IncomingEdges);
            Assert.Equal(1, node2.OutgoingEdges.Count);
            Assert.Contains(edge, node2.OutgoingEdges);
            Assert.Equal(1, node2.OutgoingEdges.Count);
            
            Assert.Contains(edge, g.Edges);
            Assert.Equal(1, g.Edges.Count);
        }

        [Fact]
        public void AddOutgoingEdge()
        {
            var g = new Graph(false);
            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = new Edge(node1, node2);
            node2.OutgoingEdges.Add(edge);

            Assert.Contains(edge, node1.IncomingEdges);
            Assert.Equal(1, node1.OutgoingEdges.Count);
            Assert.Contains(edge, node1.OutgoingEdges);
            Assert.Equal(1, node1.OutgoingEdges.Count);
            
            Assert.Contains(edge, node2.IncomingEdges);
            Assert.Equal(1, node2.OutgoingEdges.Count);
            Assert.Contains(edge, node2.OutgoingEdges);
            Assert.Equal(1, node2.OutgoingEdges.Count);
            
            Assert.Contains(edge, g.Edges);
            Assert.Equal(1, g.Edges.Count);
        }
    }
}