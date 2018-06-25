namespace Rivers.Serialization.Dot
{
    /// <summary>
    /// Represents a single token in a dot file.
    /// </summary>
    public struct DotToken
    {
        /// <summary>
        /// The text of the token.
        /// </summary>
        public string Text;
        
        /// <summary>
        /// The terminal the token is classified as.
        /// </summary>
        public DotTerminal Terminal;
        
        /// <summary>
        /// The text range the token is located at in the dot file.
        /// </summary>
        public TextRange Range;

        public DotToken(string text, DotTerminal terminal, TextRange range)
        {
            Text = text;
            Terminal = terminal;
            Range = range;
        }

        public override string ToString()
        {
            return $"({Text}, {Terminal})";
        }
    }
}