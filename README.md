Rivers
======
Rivers is a light-weight graphing library written in C#. It contains a model for directed graphs, as well as a whole bunch of standard algorithms to analyse graphs.

Rivers is released under the MIT license.

Features
========
- Easy to use graph class modelled using adjacency lists for each node. This has the following advantages:
    - Optimised for quick insertion of nodes and edges.
    - Minimal memory foodprint.
    - Efficient for sparse graphs.
- Various built-in node traversal and search algorithms (including breadth first, depth first and more).
- Built-in dominator analysis. Useful for control flow graph analysis.
    - Construct dominator trees from CFGs.
    - Get dominance frontier.
- Dot file export support. 


Quick starters guide
====================

Creating a graph
----------------

The `Graph` class contains two collections representing the nodes and edges of the graph. They work like any other collection in .NET, and you can add and remove items, and iterate through them.

```csharp
var graph = new Graph();

// Add nodes
graph.Nodes.Add(new Node("1"));
graph.Nodes.Add("2");
graph.Nodes.Add("3");

// Add edges.
graph.Edges.Add(new Edge("1", "2"));
graph.Edges.Add("2", "3");
```

Inspecting and editing nodes
----------------------------
The `Node` class has various properties that give more insight about a node in the graph. Get a node by its name through the indexer of the `Graph.Nodes` property.
- Inspect, add or remove incoming and outgoing edges on the fly using the `IncomingEdges` and `OutcomingEdges` collections.
- Store custom defined properties in the `UserData` property.

```csharp
// Obtain the node.
var node = graph.Nodes["1"];

// Add an edge from 1 to 2.
node.OutgoingEdges.Add("2");

// Set user data.
node.UserData["MyProperty"] = 1234;
```

Perform graph searches and traversals
-------------------------------------
There are various extension methods defined to perform searches in the graph. Example:

```csharp
var result = myNode.DepthFirstSearch(n => n.Name.Contains("A"));
if (result == null) 
{
    // There is no path from myNode to a node with the letter "A".
}
```

If you are more interested in the actual traversal of the nodes, you can use the corresponding `Traversal` extension instead. This will result in an `IEnumerable<Node>` that is lazily initialized with all the nodes that it is traversing.

```csharp
foreach (var node in myNode.DepthFirstTraversal()) 
{
    // ...
}

var nodes = from n in myNode.DepthFirstTraversal()
            // ...
            select n;
```