namespace Rivers.Serialization
{
    public class SyntaxError
    {
        public SyntaxError(string message, TextRange range)
        {
            Message = message;
            Range = range;
        }
        
        public string Message
        {
            get;   
        }

        public TextRange Range
        {
            get;
        }
    }
}