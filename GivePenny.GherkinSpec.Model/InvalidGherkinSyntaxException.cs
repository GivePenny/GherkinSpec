using System;

namespace GivePenny.GherkinSpec.Model
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
