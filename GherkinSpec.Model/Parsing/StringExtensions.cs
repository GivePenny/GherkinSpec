namespace GherkinSpec.Model.Parsing
{
    internal static class StringExtensions
    {
        public static string RemoveFirstAndLastPipes(this string text, int lineNumber)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (!text.StartsWith("|") || !text.EndsWith("|"))
            {
                throw new InvalidGherkinSyntaxException(
                    $"Expected line to start and end with a | character.  Actual line: \"{text}\"", lineNumber);
            }

            return text.Substring(1, text.Length - 2);
        }
    }
}
