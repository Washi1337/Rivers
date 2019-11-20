using System;
using System.IO;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Serialization.Dot
{
    public class DotWriterTest
    {     
        private static void Validate(Graph g, bool separate, bool semicolons)
        {
            var writer = new StringWriter();
            var dotWriter = new DotWriter(writer);
            dotWriter.SeparateNodesAndEdges = separate;
            dotWriter.IncludeSemicolons = semicolons;
            dotWriter.Write(g);
            
            var reader = new StringReader(writer.ToString());
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();

            Assert.Equal(g, h, new GraphComparer
            {
                IncludeUserData = true
            });
        }
        
        [Fact]
        public void EmptyGraph()
        {
            var g = new Graph(false);

            Validate(g, false, false);
        }
        
        [Fact]
        public void EmptyDiGraph()
        {
            var g = new Graph(true);

            Validate(g, false, false);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleNodes(bool semicolons)
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            Validate(g, false, semicolons);
        }
        
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void SimpleUndirectedEdges(bool separate, bool semicolons)
        {
            var g = new Graph(false);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");

            Validate(g, separate, semicolons);
        }
        
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void SimpleDirectedEdges(bool separate, bool semicolons)
        {
            var g = new Graph(true);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");

            Validate(g, separate, semicolons);
        }
        
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void EscapedIdentifiers(bool separate, bool semicolons)
        {
            var g = new Graph();
            g.Nodes.Add("A\"A");
            g.Nodes.Add("B\"B");
            g.Nodes.Add("C\"C");

            g.Edges.Add("B\"B", "C\"C");

            Validate(g, separate, semicolons);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void NodeAttributes(bool separate, bool semicolons)
        {            
            var g = new Graph();
            g.Nodes.Add("A").UserData["color"] = "red";
            g.Nodes.Add("B").UserData["color"] = "blue";
            g.Nodes.Add("C").UserData["color"] = "green";
            g.Nodes["C"].UserData["style"] = "dashed";

            Validate(g, separate, semicolons);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void EdgeAttributes(bool separate, bool semicolons)
        {            
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B").UserData["color"] = "red";
            g.Edges.Add("B", "C").UserData["color"] = "red";
            g.Nodes["B"].OutgoingEdges["C"].UserData["style"] = "dashed";

            Validate(g, separate, semicolons);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void GraphAttributes(bool separate, bool semicolons)
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            g.Edges.Add("C", "A");
            
            g.UserData["label"] = "Test";

            Validate(g, separate, semicolons);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AnonymousSubGraphs(bool directed)
        {
            var g = new Graph(directed);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            g.Edges.Add("C", "A");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["A"], g.Nodes["B"]));

            Validate(g, true, true);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NamedSubGraphs(bool directed)
        {
            var g = new Graph(directed);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            g.Edges.Add("C", "A");

            var subGraph = new SubGraph("H", g.Nodes["A"], g.Nodes["B"]);
            subGraph.UserData["color"] = "red";
            g.SubGraphs.Add(subGraph);

            Validate(g, true, true);
        }

        [Fact]
        public void IdentifierStartingWithDigit()
        {
            // https://github.com/Washi1337/Rivers/issues/7
            
            var g = new Graph(true);
            g.Nodes.Add("0x0");
            g.Nodes.Add("0x1");
            g.Edges.Add("0x0", "0x1");

            Validate(g, true, true);
        }

        [Fact]
        public void UserDataStartingWithDigit()
        {
            // https://github.com/Washi1337/Rivers/issues/7

            var g = new Graph();
            g.Nodes.Add("_0001")
                .UserData["label"] = "Add";
            g.Nodes.Add("_0002")
                .UserData["label"] = "0x70";
            g.Nodes.Add("_0003")
                .UserData["label"] = "0x70";
            
            Validate(g, true, true);
        }
    }
}