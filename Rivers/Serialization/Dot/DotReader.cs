using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Provides a mechanism for reading graphs from a character stream using the dot file format. 
    /// </summary>
    public class DotReader
    {
        // Reference:
        // https://www.graphviz.org/doc/info/lang.html
        
        private readonly DotTokenizer _tokenizer;
        private readonly Stack<Graph> _graphs = new Stack<Graph>();

        /// <summary>
        /// Creates a new dot file reader.
        /// </summary>
        /// <param name="reader">The reader responsible for reading the dot file.</param>
        public DotReader(TextReader reader)
            : this(new DotTokenizer(reader))
        {
        }

        /// <summary>
        /// Creates a new dot file reader.
        /// </summary>
        /// <param name="tokenizer">The tokenizer responsible for providing the token stream of a dot file.</param>
        public DotReader(DotTokenizer tokenizer)
        {
            _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        }

        /// <summary>
        /// Refers to the current (sub) graph to add nodes and edges to.
        /// </summary>
        private Graph CurrentGraph
        {
            get { return _graphs.Peek(); }
        }
        
        /// <summary>
        /// Reads the graph from the input stream. 
        /// </summary>
        /// <returns>The read graph</returns>
        public Graph Read()
        {
            if (_tokenizer.Peek().Terminal == DotTerminal.Strict)
            {
                // Ignore.    
                _tokenizer.Next();
            }

            var graphType = ExpectOneOf(DotTerminal.DiGraph, DotTerminal.Graph);
            bool directed = false;
            switch (graphType.Terminal)
            {
                case DotTerminal.DiGraph:
                    directed = true;
                    break;
                case DotTerminal.Graph:
                    directed = false;
                    break;
            }

            _graphs.Push(new Graph(directed));

            ExpectOneOf(DotTerminal.OpenBrace);
            ReadStatementList();
            ExpectOneOf(DotTerminal.CloseBrace);

            return _graphs.Pop();
        }

        /// <summary>
        /// Parses the stmt_list grammar rule.
        /// </summary>
        private void ReadStatementList()
        {
            // TODO: add support for attr_stmt.
            while (TryReadStatement()) ;
        }

        /// <summary>
        /// Attempts to parse the stmt grammar rule.
        /// </summary>
        /// <returns>True if it succeeded parsing the stmt rule, false otherwise.</returns>
        private bool TryReadStatement()
        {
            bool result = false;
            
            var next = TryExpectOneOf(DotTerminal.Identifier);
            
            if (next.HasValue)
            {
                // Try ID = ID rule.
                var idToken = next.Value;
                next = TryExpectOneOf(DotTerminal.Equal);
                if (next.HasValue)
                {
                    var value = ExpectOneOf(DotTerminal.Identifier);
                    CurrentGraph.UserData[idToken.Text] = value.Text;
                }
                else
                {
                    // Fall back to normal node statement.
                    var node = ReadNodeStatement(idToken.Text);
                    TryReadEdgeRhs(new[] {node});
                }

                result = true;
            }
            else
            {
                // Fall back to normal node statement.
                var nodes = ReadNextNodes(false);
                result = nodes.Count > 0;
                if (result)
                    TryReadEdgeRhs(nodes);
            }
            
            next = TryExpectOneOf(DotTerminal.SemiColon);
            if (next.HasValue)
                result = true;

            return result;
        }
        
        private ICollection<Node> ReadNextNodes(bool edgeRhs)
        {
            var nodes = new HashSet<Node>();

            var next = TryExpectOneOf(DotTerminal.SubGraph, DotTerminal.OpenBrace, DotTerminal.Identifier);

            if (next.HasValue)
            {
                switch (next.Value.Terminal)
                {
                    case DotTerminal.SubGraph:
                    case DotTerminal.OpenBrace:
                        var tempGraph = ReadSubGraph(next.Value);
                        CurrentGraph.DisjointUnionWith(tempGraph, string.Empty, true);
                        var subGraph = new SubGraph(tempGraph.Name,
                            tempGraph.Nodes.Select(x => CurrentGraph.Nodes[x.Name]).ToArray());
                        foreach (var entry in tempGraph.UserData)
                            subGraph.UserData.Add(entry.Key, entry.Value);    
                        CurrentGraph.SubGraphs.Add(subGraph);
                        nodes.UnionWith(subGraph.Nodes);
                        break;
                    case DotTerminal.Identifier:
                        nodes.Add(edgeRhs 
                            ? CurrentGraph.Nodes.Add(next.Value.Text) // In EdgeRHS rule, the target nodes can only consist of names.
                            : ReadNodeStatement(next.Value.Text)); // Otherwise, it can also contain extra attributes.
                        break;
                }
                
                TryExpectOneOf(DotTerminal.SemiColon);
            }

            return nodes;
        }

        /// <summary>
        /// Parses the subgraph grammar rule. This function assumes the first token indicating the subgraph is alread consumed.
        /// </summary>
        /// <param name="start">The starting token identifying the subgraph.</param>
        private Graph ReadSubGraph(DotToken start)
        {
            string name = null;
            
            if (start.Terminal == DotTerminal.SubGraph)
            {
                var next = ExpectOneOf(DotTerminal.Identifier, DotTerminal.OpenBrace);
                if (next.Terminal == DotTerminal.Identifier)
                {
                    name = next.Text;
                    ExpectOneOf(DotTerminal.OpenBrace);
                }
            }

            _graphs.Push(new Graph(CurrentGraph.IsDirected) { Name = name });
            ReadStatementList();
            ExpectOneOf(DotTerminal.CloseBrace);

            return _graphs.Pop();
        }

        /// <summary>
        /// Parses the node_stmt grammar rule. This function assumes the actual node token is already consumed.
        /// </summary>
        /// <param name="name">The node name.</param>
        private Node ReadNodeStatement(string name)
        {
            var node = CurrentGraph.Nodes.Add(name);

            var attributes = TryReadAttributeList();
            if (attributes != null)
            {
                foreach (var entry in attributes)
                    node.UserData[entry.Key] = entry.Value;
            }

            return node;
        }

        /// <summary>
        /// Attempts to parse the edge_rhs grammar rule. This function assumes the tokens representing the source nodes
        /// are already consumed.
        /// </summary>
        /// <param name="sourceNodes">The source nodes to draw edges from.</param>
        /// <returns>True if it succeeded, false otherwise.</returns>
        private void TryReadEdgeRhs(ICollection<Node> sourceNodes)
        {
            var edgeOpTerminal = CurrentGraph.IsDirected ? DotTerminal.DirectedEdge : DotTerminal.UndirectedEdge;
            while ((TryExpectOneOf(edgeOpTerminal)).HasValue)
            {
                var targets = ReadNextNodes(true);

                foreach (var target in targets)
                {
                    var attributes = TryReadAttributeList();

                    foreach (var sourceNode in sourceNodes)
                    {
                        var edge = new Edge(sourceNode, target);
                        CurrentGraph.Edges.Add(edge);
                        if (attributes != null)
                        {
                            foreach (var entry in attributes)
                                edge.UserData[entry.Key] = entry.Value;
                        }
                    }
                }

                sourceNodes = targets;
            }
        }
        
        /// <summary>
        /// Attempts to parse the attr_list grammar rule.
        /// </summary>
        /// <returns>A dictionary containing the attributes and their values, or null if it failed.</returns>
        private IDictionary<string, string> TryReadAttributeList()
        {
            var open = TryExpectOneOf(DotTerminal.OpenBracket);
            if (open == null)
                return null;

            var properties = new Dictionary<string, string>();

            do
            {
                var property = ExpectOneOf(DotTerminal.Identifier);
                ExpectOneOf(DotTerminal.Equal);
                var value = ExpectOneOf(DotTerminal.Identifier);

                properties[property.Text] = value.Text;
            } while (TryExpectOneOf(DotTerminal.Comma, DotTerminal.SemiColon) != null);

            ExpectOneOf(DotTerminal.CloseBracket);
            return properties;
        }

        /// <summary>
        /// Attempts to consume one of the provided terminals.
        /// If it fails, the tokenizer does not consume anything. 
        /// </summary>
        /// <param name="terminals">The terminals to choose from.</param>
        /// <returns>The read token, or null if no terminal was matched and no token was consumed.</returns>
        private DotToken? TryExpectOneOf(params DotTerminal[] terminals)
        {
            var token = _tokenizer.Peek();
            if (!terminals.Contains(token.Terminal))
                return null;
            return _tokenizer.Next();
        }

        /// <summary>
        /// Consumes one of the provided terminals.
        /// </summary>
        /// <param name="terminals">The terminals to choose from.</param>
        /// <returns>The consumed token.</returns>
        /// <exception cref="SyntaxException">Occurs when no terminal was matched.</exception>
        private DotToken ExpectOneOf(params DotTerminal[] terminals)
        {
            var token = _tokenizer.Next();
            if (!terminals.Contains(token.Terminal))
                throw new SyntaxException(new SyntaxError("Expected " + string.Join(", ", terminals), token.Range));
            return token;
        }
    }
}