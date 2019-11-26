
using Rivers.Analysis.Traversal;

using Xunit;

namespace Rivers.Test.Analysis.Traversal
{
    public class DepthTest
    {
        private static readonly Graph Tree;

        static DepthTest()
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
        public void BreadthFirstDepthShouldBeZeroOnFirstNode()
        {
            // Arrange
            var start = new Node("7");
            Tree.Nodes.Add(start);

            var traversal = new BreadthFirstTraversal();
            traversal.NodeDiscovered += (sender, args) =>
            {
                Assert.Equal(0, args.Depth);
            };

            // Act
            traversal.Run(start);

            // Assert done in NodeDiscoverd event handler.
        }

        [Fact]
        public void BreadthDepthShouldBeTwoWhenEndNodeIsFoundTest()
        {
            // Arrange
            var start = Tree.Nodes["1"];
            var stop = Tree.Nodes["5"];

            var traversal = new BreadthFirstTraversal();
            Node result = null;
            int depth = 0;
            traversal.NodeDiscovered += (sender, args) =>
            {
                if (args.NewNode == stop)
                {
                    args.ContinueExploring = false;
                    args.Abort = true;
                    result = args.NewNode;
                    depth = args.Depth;
                }
            };

            // Act
            traversal.Run(start);

            // Assert
            Assert.Equal(stop, result);
            Assert.Equal(2, depth);
        }

        [Fact]
        public void DepthFirstDepthShouldBeZeroOnFirstNodeTest()
        {
            // Arrange
            var start = new Node("7");
            Tree.Nodes.Add(start);

            var traversal = new DepthFirstTraversal();
            traversal.NodeDiscovered += (sender, args) =>
            {
                Assert.Equal(0, args.Depth);
            };

            // Act
            traversal.Run(start);

            // Assert done in NodeDiscoverd event handler.
        }

        [Fact]
        public void DepthFirstDepthShouldBeTwoWhenEndNodeIsFoundTest()
        {
            // Arrange
            var start = Tree.Nodes["1"];
            var stop = Tree.Nodes["5"];

            var traversal = new DepthFirstTraversal();
            Node result = null;
            int depth = 0;
            traversal.NodeDiscovered += (sender, args) =>
            {
                if (args.NewNode == stop)
                {
                    args.ContinueExploring = false;
                    args.Abort = true;
                    result = args.NewNode;
                    depth = args.Depth;
                }
            };

            // Act
            traversal.Run(start);

            // Assert
            Assert.Equal(stop, result);
            Assert.Equal(2, depth);
        }
    }
}