using System;
using System.Collections.Generic;
using System.Linq;

namespace Rivers.Serialization
{
    public class SyntaxException : Exception
    {
        public SyntaxException(params SyntaxError[] syntaxErrors)
            : this(syntaxErrors.AsEnumerable())
        {
        }

        public SyntaxException(IEnumerable<SyntaxError> syntaxErrors)
            : base("Input file contains syntax errors.")
        {
            SyntaxErrors = new List<SyntaxError>(syntaxErrors);
        }
        
        public ICollection<SyntaxError> SyntaxErrors
        {
            get;
        }
    }
}