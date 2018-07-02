using Xunit;

namespace Rivers.Test
{
    public class EdgeCollectionTest
    {
        [Fact]
        public void EmptyCollection()
        {
            var g = new Graph();
            Assert.Equal(0, g.Edges.Count);
        }

        [Fact]
        public void AddEdgeToAddedNodes()
        {
            var g = new Graph();

            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = g.Edges.Add(node1, node2);
            Assert.Contains(edge, g.Edges);
            Assert.Contains(edge, node1.OutgoingEdges);
            Assert.Contains(edge, node2.IncomingEdges);
        }

        [Fact]
        public void AddEdgeTwice()
        {
            var g = new Graph();

            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge1 = g.Edges.Add(node1, node2);
            var edge2 = g.Edges.Add(node1, node2);
            Assert.Equal(1, g.Edges.Count);
            Assert.Equal(edge1, edge2);
        }

        [Fact]
        public void RemoveEdge()
        {
            var g = new Graph();

            var node1 = g.Nodes.Add("1");
            var node2 = g.Nodes.Add("2");

            var edge = g.Edges.Add(node1, node2);
            g.Edges.Remove(edge);

            Assert.Empty(g.Edges);
            Assert.Empty(node1.OutgoingEdges);
            Assert.Empty(node2.IncomingEdges);
        }
    }
}