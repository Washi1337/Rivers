using Rivers.Analysis;
using Xunit;

namespace Rivers.Test.Analysis
{
    public class CategorizationTest
    {
        [Fact]
        public void CyclicDirectedTest()
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
        public void CyclicDirectedTest2Nodes()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Edges.Add("1", "2");
            g.Edges.Add("2", "1");
            Assert.True(g.IsCyclic());
        }

        [Fact]
        public void AcyclicDirectedTest()
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
        
        [Fact]
        public void CyclicUndirectedTest()
        {
            var g = new Graph(false);
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
        public void AcyclicUndirectedTest()
        {
            var g = new Graph(false);
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
        
        [Fact]
        public void AcyclicDirectedTest2Nodes()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Edges.Add("1", "2");
            Assert.False(g.IsCyclic());
        }


        [Fact]
        public void ConnectedDirectedTest()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");

            g.Edges.Add("1", "2");
            g.Edges.Add("2", "3");
            g.Edges.Add("1", "4");
            
            Assert.True(g.IsConnected());
        }
        
        [Fact]
        public void ConnectedReversedDirectedTest()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");

            g.Edges.Add("3", "2");
            g.Edges.Add("2", "1");
            g.Edges.Add("1", "4");
            
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void DisconnectedDirectedTest()
        {
            var g = new Graph();
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");

            g.Edges.Add("1", "2");
            g.Edges.Add("2", "3");
            
            Assert.False(g.IsConnected());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DirectedTree(bool directed)
        {
            var g = new Graph(directed);
            g.Nodes.Add("1");
            g.Nodes.Add("2");
            g.Nodes.Add("3");
            g.Nodes.Add("4");
            g.Nodes.Add("5");

            g.Edges.Add("1", "2");
            g.Edges.Add("1", "3");
            g.Edges.Add("2", "4");
            g.Edges.Add("2", "5");
            
            Assert.True(g.IsTree());
        }
        
        
    }
}