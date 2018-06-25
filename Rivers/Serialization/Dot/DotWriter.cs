using System;
using System.Collections.Generic;
using System.IO;

namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Provides a mechanism for writing graphs to a character stream using the dot file format.
    /// </summary>
    public class DotWriter
    {
        public static readonly IDictionary<char, string> EscapedCharacters = new Dictionary<char, string>
        {
            ['\r'] = "\\\r",
            ['\n'] = "\\\n",
            ['"'] = "\\\"",
            ['\t'] = "\\\t",
        };
        
        private readonly TextWriter _writer;

        public DotWriter(TextWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        /// <summary>
        /// Writes a graph to the character stream.
        /// </summary>
        /// <param name="graph">The graph to write.</param>
        public void Write(Graph graph)
        {
            WriteHeader();
            
            foreach (var node in graph.Nodes)
                Write(node);
            
            foreach (var edge in graph.Edges)
                Write(edge);

            WriteFooter();
        }

        private void WriteHeader()
        {
            _writer.WriteLine("strict digraph {");
        }

        private void WriteFooter()
        {
            _writer.WriteLine("}");
        }

        private void Write(Node node)
        {
            WriteString(node.Name);

            if (node.UserData.Count > 0)
            {
                _writer.Write(" [");
                WriteCommaSeparatedString(node.UserData);
                _writer.Write(']');
            }

            _writer.WriteLine();
        }

        private void Write(Edge edge)
        {
            WriteString(edge.Source.Name);
            _writer.Write(" -> ");
            WriteString(edge.Target.Name);
            
            if (edge.UserData.Count > 0)
            {
                _writer.Write(" [");
                WriteCommaSeparatedString(edge.UserData);
                _writer.Write(']');
            }
            
            _writer.WriteLine();
        }

        private void WriteCommaSeparatedString(ICollection<KeyValuePair<object, object>> objects)
        {
            int c = 0;
            foreach (var entry in objects)
            {
                _writer.Write(entry.Key.ToString());
                _writer.Write('=');
                WriteString(entry.Value.ToString());
                if (c < objects.Count - 1)
                    _writer.Write(", ");
                c++;
            }   
        }

        private void WriteString(string text)
        {
            _writer.Write('"');
            foreach (var c in text)
                WriteEscapedCharacter(c);
            _writer.Write('"');
        }

        private void WriteEscapedCharacter(char c)
        {
            if (EscapedCharacters.TryGetValue(c, out string escaped))
                _writer.Write(escaped);
            else
                _writer.Write(c);
        }
        
    }
}