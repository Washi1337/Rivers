using System.Collections.Generic;
using System.IO;
using Rivers.Serialization;
using Rivers.Serialization.Dot;
using Xunit;

namespace Rivers.Test.Serialization.Dot
{
    public class DotReaderTest
    {
        [Fact]
        public void EmptyDiGraph()
        {
            var g = new Graph(true);
            
            var reader = new StringReader("strict digraph { }");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void EmptyGraph()
        {
            var g = new Graph(false);
            
            var reader = new StringReader("strict graph { }");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void SimpleNodes()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");
            
            var reader = new StringReader(
                @"strict digraph { 
    A
    B
    C

}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void SimpleDirectedEdges()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            
            var reader = new StringReader(
                @"strict digraph { 
    A
    B
    C

    A -> B
    B -> C
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void SimpleUndirectedEdges()
        {
            var g = new Graph(false);
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            
            var reader = new StringReader(
                @"strict graph { 
    A
    B
    C

    A -- B
    B -- C
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void UndirectedEdgesInDirectedGraph()
        {
            var reader = new StringReader(
                @"strict digraph { 
    A
    B
    C

    A -- B
    B -- C
}");
            var dotReader = new DotReader(reader);
            Assert.Throws<SyntaxException>(() => dotReader.Read());
        }
        
        [Fact]
        public void OnlyEdges()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            
            var reader = new StringReader(
                @"strict digraph { 
    A -> B
    B -> C
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }
        
        [Fact]
        public void EscapedIdentifiers()
        {
            var g = new Graph();
            g.Nodes.Add("A\"A");
            g.Nodes.Add("B\"B");
            g.Nodes.Add("C\"C");

            g.Edges.Add("B\"B", "C\"C");
            
            var reader = new StringReader(
                @"strict digraph { 
""A\""A""
""B\""B"" -> ""C\""C""
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer());
        }

        [Fact]
        public void NodeAttributes()
        {            
            var g = new Graph();
            g.Nodes.Add("A").UserData["color"] = "red";
            g.Nodes.Add("B").UserData["color"] = "blue";
            g.Nodes.Add("C").UserData["color"] = "green";
            g.Nodes["C"].UserData["style"] = "dashed";
            
            var reader = new StringReader(
                @"strict digraph { 
A [color=red]
B [color=blue]
C [color=green, style=dashed]
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void EdgeAttributes()
        {            
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B").UserData["color"] = "red";
            g.Edges.Add("B", "C").UserData["color"] = "red";
            g.Nodes["B"].OutgoingEdges["C"].UserData["style"] = "dashed";
            
            var reader = new StringReader(
                @"strict digraph { 
A -> B [color=red]
B -> C [color=green, style=dashed]
}");
            var dotReader = new DotReader(reader);

            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void ShortSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");
            g.Nodes.Add("D");

            g.Edges.Add("A", "B");
            g.Edges.Add("C", "D");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["A"], g.Nodes["B"]));
            
            var reader = new StringReader(
                @"strict digraph { 

{ A -> B }

C -> D

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void NodeChain()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            
            var reader = new StringReader(
                @"strict digraph { 

A -> B -> C

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void AnonymousSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");
            g.Nodes.Add("D");

            g.Edges.Add("A", "B");
            g.Edges.Add("C", "D");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["A"], g.Nodes["B"]));
            
            var reader = new StringReader(
                @"strict digraph { 

subgraph { A -> B }

C -> D

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeSubGraphs = true,
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }
        
        [Fact]
        public void NamedSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");
            g.Nodes.Add("D");

            g.Edges.Add("A", "B");
            g.Edges.Add("C", "D");
            
            g.SubGraphs.Add(new SubGraph("H", g.Nodes["A"], g.Nodes["B"]));
            
            var reader = new StringReader(
                @"strict digraph { 

subgraph H { A -> B }

C -> D

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeSubGraphs = true,
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void NodeToSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("A", "C");
            g.Edges.Add("B", "C");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["B"], g.Nodes["C"]));
            
            var reader = new StringReader(
                @"strict digraph { 

A -> { B -> C }

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeSubGraphs = true,
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void SubGraphToNode()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("B", "C");
            g.Edges.Add("B", "A");
            g.Edges.Add("C", "A");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["B"], g.Nodes["C"]));
            
            var reader = new StringReader(
                @"strict digraph { 

{ B -> C } -> A

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeSubGraphs = true,
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void SubGraphToSubGraph()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");
            g.Nodes.Add("D");

            g.Edges.Add("A", "B");
            g.Edges.Add("A", "C");
            g.Edges.Add("A", "D");
            g.Edges.Add("B", "C");
            g.Edges.Add("B", "D");
            g.Edges.Add("C", "D");
            
            g.SubGraphs.Add(new SubGraph(g.Nodes["A"], g.Nodes["B"]));
            g.SubGraphs.Add(new SubGraph(g.Nodes["C"], g.Nodes["D"]));
            
            var reader = new StringReader(
                @"strict digraph { 

{ A -> B } -> { C -> D } 

}");
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeSubGraphs = true,
                NodeComparer = new NodeComparer() {IncludeUserData = true}
            });
        }

        [Fact]
        public void GraphAttributes()
        {
            var g = new Graph();
            g.Nodes.Add("A");
            g.Nodes.Add("B");
            g.Nodes.Add("C");

            g.Edges.Add("A", "B");
            g.Edges.Add("B", "C");
            g.Edges.Add("C", "A");

            g.UserData["color"] = "blue";
            g.UserData["label"] = "Test";
            
            var reader = new StringReader(
                @"strict digraph {
color=blue
A -> B -> C -> A
label=Test
}");
            
            var dotReader = new DotReader(reader);
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                IncludeUserData = true
            });
        }

        [Fact]
        public void CustomSerializer()
        {
            var g = new Graph();
            var a = g.Nodes.Add("A");
            var b = g.Nodes.Add("B");

            a.UserData["MyIntegerList"] = new List<int> {1, 2, 3};
            b.UserData["MyProperty"] = "SomeValue";
            
            var reader = new StringReader(
                @"strict digraph {
A [MyIntegerList=""1,2,3""]
B [MyProperty=""SomeValue""]
}");
         
            // stuff is happening here.
            var dotReader = new DotReader(reader, new CustomUserDataSerializer());
            var h = dotReader.Read();
            Assert.Equal(g, h, new GraphComparer()
            {
                NodeComparer = new NodeComparer { IncludeUserData = true }
            });   
        }
    }
}