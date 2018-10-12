using System;
using Xunit;

namespace Rivers.Test
{
    public class EmptyAdjacencyCollectionTest
    {
        [Fact]
        public void EmptyCollection()
        {
            var node = new Node("1");
            
            Assert.Empty(node.IncomingEdges);
            Assert.Empty(node.OutgoingEdges);
        }

        [Fact]
        public void AddEdge()
        {
            var node1 = new Node("1");
            var node2 = new Node("2");

            var edge = new Edge(node1, node2);
            Assert.Throws<InvalidOperationException>(() => node2.IncomingEdges.Add(edge));
        }
    }
}