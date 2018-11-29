using System;

namespace GivePenny.GherkinCore.Model
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
