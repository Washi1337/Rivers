using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            WriteIdentifier(node.Name);

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
            WriteIdentifier(edge.Source.Name);
            _writer.Write(" -> ");
            WriteIdentifier(edge.Target.Name);
            
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
                WriteIdentifier(entry.Key.ToString());
                _writer.Write('=');
                WriteIdentifier(entry.Value.ToString());
                if (c < objects.Count - 1)
                    _writer.Write(", ");
                c++;
            }   
        }

        private void WriteIdentifier(string text)
        {
            if (!NeedsEscaping(text))
            {
                _writer.Write(text);
            }
            else
            {
                _writer.Write('"');
                foreach (var c in text)
                    WriteEscapedCharacter(c);
                _writer.Write('"');
            }
        }

        private static bool NeedsEscaping(string text)
        {
            return text.Any(c => EscapedCharacters.ContainsKey(c));
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