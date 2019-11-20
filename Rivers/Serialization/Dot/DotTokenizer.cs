using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Represents a lexer that tokenizes files in the dot file format.
    /// </summary>
    public class DotTokenizer
    {
        private static readonly IDictionary<string, DotTerminal> Keywords = new Dictionary<string, DotTerminal>(StringComparer.OrdinalIgnoreCase)
        {
            ["strict"] = DotTerminal.Strict,
            ["graph"] = DotTerminal.Graph,
            ["digraph"] = DotTerminal.DiGraph,
            ["subgraph"] = DotTerminal.SubGraph,
        };
        
        private readonly TextReader _reader;
        private TextLocation _startLocation; 
        private TextLocation _currentLocation;
        private DotToken? _bufferedToken;

        public DotTokenizer(TextReader reader)
        {   
            _reader = reader;
            _currentLocation = new TextLocation(1, 1);
        }

        /// <summary>
        /// Determines whether the tokenizer has reached the end of the character stream.
        /// </summary>
        /// <returns>True if it has reached the end, false otherwise.</returns>
        public bool HasNext()
        {
            if (_bufferedToken == null)
            {
                SkipWhitespaces();
                return _reader.Peek() == -1;
            }

            return true;
        }

        /// <summary>
        /// Reads the next token, but does not consume it.
        /// </summary>
        /// <returns>The token.</returns>
        public DotToken Peek()
        {
            if (_bufferedToken == null)
                _bufferedToken = ReadNextToken();

            return _bufferedToken.Value;
        }

        /// <summary>
        /// Reads and consumes the next token.
        /// </summary>
        /// <returns></returns>
        public DotToken Next()
        {
            var token = Peek();
            _bufferedToken = null;
            return token;
        }

        private DotToken ReadNextToken()
        {
            SkipWhitespaces();
            _startLocation = _currentLocation;
            
            int pc = _reader.Peek();
            if (pc == -1)
                throw new EndOfStreamException();
            char c = (char) pc;

            if (char.IsDigit(c))
            {
                string integer = ReadIntegerIdentifier();
                return new DotToken(integer, DotTerminal.Identifier, new TextRange(_startLocation, _currentLocation));
            }
            else if (IsWordStart(c))
            {
                string word = ReadWordIdentifier();
                if (!Keywords.TryGetValue(word, out var terminal))
                    terminal = DotTerminal.Identifier;
                return new DotToken(word, terminal, new TextRange(_startLocation, _currentLocation));
            }
            
            switch (c)
            {
                case '\"':
                    return ReadEnclosedIdentifier();
                case '{':
                    return ReadCharacterToken(DotTerminal.OpenBrace);
                case '}':
                    return ReadCharacterToken(DotTerminal.CloseBrace);
                case '[':
                    return ReadCharacterToken(DotTerminal.OpenBracket);
                case ']':
                    return ReadCharacterToken(DotTerminal.CloseBracket);
                case ',':
                    return ReadCharacterToken(DotTerminal.Comma);
                case '=':
                    return ReadCharacterToken(DotTerminal.Equal);
                case ';':
                    return ReadCharacterToken(DotTerminal.SemiColon);
                case '-':
                    return ReadEdgeToken();
                default:
                    return UnrecognisedToken();
            }
        }

        private static bool IsWordLetter(char c)
        {
            return IsWordStart(c) || char.IsDigit(c);
        }

        private static bool IsWordStart(char c)
        {
            return char.IsLetter(c) || c == '_';
        }
        
        private DotToken UnrecognisedToken()
        {
            return new DotToken("Unrecognized token.", DotTerminal.Error,
                new TextRange(_startLocation, _currentLocation));
        }

        private void SkipWhitespaces()
        {
            while (true)
            {
                int c = _reader.Peek();
                if (c == -1 || !char.IsWhiteSpace((char) c))
                    break;
                ReadCharacter();
            }
        }

        private char ReadCharacter()
        {
            char c = (char) _reader.Read();
            _currentLocation = c == '\n'
                ? _currentLocation = new TextLocation(_currentLocation.Line + 1, 1)
                : _currentLocation.Offset(0, 1);
            return c;
        }

        private string ReadIntegerIdentifier()
        {
            return ReadIdentifier(char.IsDigit);
        }

        private string ReadWordIdentifier()
        {
            return ReadIdentifier(IsWordLetter);
        }

        private string ReadIdentifier(Predicate<char> condition)
        {
            var builder = new StringBuilder();
            
            while(true)
            {
                int c = _reader.Peek();
                if (c == -1 || !condition((char) c))
                    break;
                builder.Append(ReadCharacter());
            }

            return builder.ToString();
        }
        
        private DotToken ReadCharacterToken(DotTerminal terminal)
        {
            return new DotToken(ReadCharacter().ToString(), terminal,
                new TextRange(_startLocation, _currentLocation)); 
        }
        
        private DotToken ReadEnclosedIdentifier()
        {            
            var builder = new StringBuilder();

            if (ReadCharacter() != '"')
                throw new InvalidOperationException();
            
            bool isEscaping = false;
            bool end = false;
            
            while(!end)
            {
                int pc = _reader.Peek();
                if (pc == -1)
                {
                    return new DotToken("Expected \".", DotTerminal.Error,
                        new TextRange(_startLocation, _currentLocation));
                }

                char c = ReadCharacter();
                
                switch (c)
                {
                    case '\\':
                        if (isEscaping)
                            builder.Append('\\');
                        isEscaping = !isEscaping;
                        break;
                    case 'r':
                        builder.Append(isEscaping ? '\r' : c);
                        isEscaping = false;
                        break;
                    case 'n':
                        builder.Append(isEscaping ? '\n' : c);
                        isEscaping = false;
                        break;
                    case 't':
                        builder.Append(isEscaping ? '\t' : c);
                        isEscaping = false;
                        break;
                    case '"':
                        if (!isEscaping)
                            end = true;
                        else
                            builder.Append('"');
                        isEscaping = false;
                        break;
                    default:
                        builder.Append(c);
                        isEscaping = false;
                        break;
                }
            }

            return new DotToken(builder.ToString(), DotTerminal.Identifier,
                new TextRange(_startLocation, _currentLocation));
        }

        private DotToken ReadEdgeToken()
        {
            char c;
            c = ReadCharacter();
            var c2 = ReadCharacter();
            string op = new string(new[] {c, c2});
            DotTerminal terminal;
            switch (c2)
            {
                case '-':
                    terminal = DotTerminal.UndirectedEdge;
                    break;
                case '>':
                    terminal = DotTerminal.DirectedEdge;
                    break;
                default:
                    terminal = DotTerminal.Error;
                    break;
            }

            if (terminal != DotTerminal.Error)
                return new DotToken(op, terminal, new TextRange(_startLocation, _currentLocation));
            return UnrecognisedToken();
        }
    }
}