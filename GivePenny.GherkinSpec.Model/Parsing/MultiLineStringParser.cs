using System.Text;

namespace GivePenny.GherkinSpec.Model.Parsing
{
    internal static class MultiLineStringParser
    {
        public static string ParseMultiLineStringIfPresent(LineReader reader)
        {
            if (!reader.IsMultiLineStringStartOrEndLine)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();

            var blockIndentation = reader.CurrentLineIndentation;
            reader.ReadNextLineRaw();

            while (!reader.IsMultiLineStringStartOrEndLine)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.AppendLine();
                }

                var line = reader.CurrentLineUntrimmed;
                if (!line.StartsWith(blockIndentation))
                {
                    throw new InvalidGherkinSyntaxException(
                        $"All lines of a multi-line string must have the same indentation as the starting quote marks, or must be indented further. Line {reader.CurrentLineNumber} contains indentation that is less than the opening quote mark line. This exception can also be thrown if there are different tabs and spaces on different lines of the block.",
                        reader.CurrentLineNumber);
                }

                line = line.Substring(blockIndentation.Length);
                stringBuilder.Append(line);

                reader.ReadNextLineRaw();
            }

            reader.ReadNextLine();

            return stringBuilder.ToString();
        }
    }
}
