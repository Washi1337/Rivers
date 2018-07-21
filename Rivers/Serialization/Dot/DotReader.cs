using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Provides a mechanism for reading graphs from a character stream using the dot file format. 
    /// </summary>
    public class DotReader
    {
        // https://www.graphviz.org/doc/info/lang.html
        
        private readonly DotTokenizer _tokenizer;
        private readonly Stack<Graph> _graphs = new Stack<Graph>();

        public DotReader(TextReader reader)
        {
            if (reader == null) 
                throw new ArgumentNullException(nameof(reader));
            
            _tokenizer = new DotTokenizer(reader);
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
        /// <exception cref="NotSupportedException">Occurs when the dot file contains an undirected graph.</exception>
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
            // TODO: add support for attr_stmt and subgraph.

            while (true)
            {
                var next = TryExpectOneOf(DotTerminal.SubGraph, DotTerminal.OpenBrace);
                if (next != null)
                {
                    ReadSubGraph(next.Value);
                }
                else
                {
                    next = TryExpectOneOf(DotTerminal.Identifier);
                    if (next == null)
                        break;

                    if (!TryReadEdgeStatement(next.Value.Text))
                        ReadNodeStatement(next.Value.Text);
                }
                 
                TryExpectOneOf(DotTerminal.SemiColon);
            }   
        }

        /// <summary>
        /// Parses the subgraph grammar rule. This function assumes the first token indicating the subgraph is alread consumed.
        /// </summary>
        /// <param name="start">The starting token identifying the subgraph.</param>
        private void ReadSubGraph(DotToken start)
        {
            if (start.Terminal == DotTerminal.SubGraph)
            {
                var next = ExpectOneOf(DotTerminal.Identifier, DotTerminal.OpenBrace);
                if (next.Terminal == DotTerminal.Identifier)
                    ExpectOneOf(DotTerminal.OpenBrace);
            }

            _graphs.Push(new Graph());
            ReadStatementList();
            var subGraph = _graphs.Pop();

            CurrentGraph.DisjointUnionWith(subGraph, string.Empty, true);
            
            ExpectOneOf(DotTerminal.CloseBrace);
        }

        /// <summary>
        /// Parses the node_stmt grammar rule. This function assumes the actual node token is already consumed.
        /// </summary>
        /// <param name="name">The node name.</param>
        private void ReadNodeStatement(string name)
        {
            var node = CurrentGraph.Nodes.Add(name);

            var attributes = TryReadAttributeList();
            if (attributes != null)
            {
                foreach (var entry in attributes)
                    node.UserData[entry.Key] = entry.Value;
            }
        }

        /// <summary>
        /// Attempts to parse the edge_stmt grammar rule. This function assumes the token containing the name of the
        /// source node is already consumed.
        /// </summary>
        /// <param name="sourceName">The name of the source node.</param>
        /// <returns>True if it succeeded, false otherwise.</returns>
        private bool TryReadEdgeStatement(string sourceName)
        {
            var edgeOp = TryExpectOneOf(CurrentGraph.IsDirected ? DotTerminal.DirectedEdge : DotTerminal.UndirectedEdge);
            if (edgeOp == null)
                return false;
            
            var startNode = CurrentGraph.Nodes.Add(sourceName);

            var target = ExpectOneOf(DotTerminal.Identifier);
            var targetNode = CurrentGraph.Nodes.Add(target.Text);
            
            var edge = new Edge(startNode, targetNode);
            CurrentGraph.Edges.Add(edge);

            var attributes = TryReadAttributeList();
            if (attributes != null)
            {
                foreach (var entry in attributes)
                    edge.UserData[entry.Key] = entry.Value;
            }

            return true;
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