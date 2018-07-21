using System;
using System.IO;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Serialization.Dot
{
    public class DotWriterTest
    {     
        private static void Validate(Graph g, bool separate)
        {
            var writer = new StringWriter();
            var dotWriter = new DotWriter(writer);
            dotWriter.SeparateNodesAndEdges = separate;
            dotWriter.Write(g);
            
            var reader = new StringReader(writer.ToString());
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();

            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void EmptyGraph()
        {
            var g = new Graph(false);

            Validate(g, false);
        }
        
        [Fact]
        public void EmptyDiGraph()
        {
            var g = new Graph(true);

            Validate(g, false);
        }

        [Fact]
        public void SimpleNodes()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            Validate(g, false);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleUndirectedEdges(bool separate)
        {
            var g = new Graph(false);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");

            Validate(g, separate);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleDirectedEdges(bool separate)
        {
            var g = new Graph(true);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");

            Validate(g, separate);
        }
        
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EscapedIdentifiers(bool separate)
        {
            var g = new Graph();
            g.Nodes.Add("A\"A");
            g.Nodes.Add("B\"B");
            g.Nodes.Add("C\"C");

            g.Edges.Add("B\"B", "C\"C");

            Validate(g, separate);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NodeAttributes(bool separate)
        {            
            var g = new Graph();
            g.Nodes.Add("A").UserData["color"] = "red";
            g.Nodes.Add("B").UserData["color"] = "blue";
            g.Nodes.Add("C").UserData["color"] = "green";
            g.Nodes["C"].UserData["style"] = "dashed";

            Validate(g, separate);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void EdgeAttributes(bool separate)
        {            
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B").UserData["color"] = "red";
            g.Edges.Add("B", "C").UserData["color"] = "red";
            g.Nodes["B"].OutgoingEdges["C"].UserData["style"] = "dashed";

            Validate(g, separate);
        }
    }
}