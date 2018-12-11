using System;

namespace GherkinSpec.Model.Parsing
{
    public class InvalidGherkinSyntaxException : Exception
    {
        public InvalidGherkinSyntaxException(string message, int lineNumber)
            : base(message)
        {
            LineNumber = lineNumber;
        }

        public int LineNumber { get; }
    }
}
