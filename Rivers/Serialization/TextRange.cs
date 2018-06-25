namespace Rivers.Serialization
{
    /// <summary>
    /// Represents a range of text in a text document.
    /// </summary>
    public struct TextRange
    {
        public static readonly TextRange Empty = new TextRange(TextLocation.Empty, TextLocation.Empty);

        public TextRange(int startLine, int startColumn, int endLine, int endColumn)
            : this(new TextLocation(startLine, startColumn), new TextLocation(endLine, endColumn))
        {
        }

        public TextRange(TextLocation start, TextLocation end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Gets the start location of the text range.
        /// </summary>
        public TextLocation Start
        {
            get;
        }
        
        /// <summary>
        /// Gets the end location of the text range.
        /// </summary>
        public TextLocation End
        {
            get;
        }

        /// <summary>
        /// Determines whether the given text location is inbetween the boundaries of this text range.
        /// </summary>
        /// <param name="location">The location to check.</param>
        /// <returns><c>True</c> if the location is inbetween the boundaries, otherwise <c>False</c>.</returns>
        public bool Contains(TextLocation location)
        {
            return location >= Start && location <= End;
        }

        /// <summary>
        /// Determines whether the given text range boundaries is inbetween the boundaries of this text range.
        /// </summary>
        /// <param name="range">The range to check.</param>
        /// <returns><c>True</c> if the range is inbetween the boundaries, otherwise <c>False</c>.</returns>
        public bool Contains(TextRange range)
        {
            return Contains(range.Start) && Contains(range.End);
        }

        /// <summary>
        /// Determines whether the given text range intersects with this text range.
        /// </summary>
        /// <param name="range">The range to check.</param>
        /// <returns><c>True</c> if the two ranges intersect, otherwise <c>False</c>.</returns>
        public bool IntersectsWith(TextRange range)
        {
            return Contains(range.Start) || Contains(range.End);
        }

        /// <summary>
        /// Translates this text range by a given amount of lines and columns.
        /// </summary>
        /// <param name="deltaLine">The line translation to use.</param>
        /// <param name="deltaColumn">The column translation to use.</param>
        /// <returns>The translated text range.</returns>
        public TextRange Offset(int deltaLine, int deltaColumn)
        {
            return new TextRange(Start.Offset(deltaLine, deltaColumn), End.Offset(deltaLine, deltaColumn));
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TextRange)
                return (TextRange)obj == this;
            return false;
        }

        public static bool operator ==(TextRange a, TextRange b)
        {
            return a.Start == b.Start && a.End == b.End;
        }

        public static bool operator !=(TextRange a, TextRange b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", Start, End);
        }
    }
}