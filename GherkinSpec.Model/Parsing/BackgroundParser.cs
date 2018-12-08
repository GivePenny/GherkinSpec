using System;
using System.Collections.Generic;

namespace GherkinSpec.Model.Parsing
{
    static class BackgroundParser
    {
        public static Background ParseBackgroundIfPresent(LineReader reader)
        {
            if (reader.IsBackgroundStartLine)
            {
                return ParseBackground(reader);
            }

            return Background.Empty;
        }

        private static Background ParseBackground(LineReader reader)
        {
            var startingLineNumber = reader.CurrentLineNumber;
            var steps = new List<GivenStep>();

            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                if (!(reader.IsAndLine || reader.IsButLine || reader.IsGivenLine))
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Backgrounds can only contain Given, And or But steps.  Found '{reader.CurrentLineTrimmed}' instead on line {reader.CurrentLineNumber}.",
                        reader.CurrentLineNumber);
                }

                steps.Add(
                    ParseBackgroundStep(reader));
            }

            return new Background(steps, startingLineNumber);
        }

        private static GivenStep ParseBackgroundStep(LineReader reader)
        {
            if (!reader.IsGivenLine && !reader.IsAndLine && !reader.IsButLine)
            {
                throw new NotSupportedException(
                    $"Unrecognised step type on line '{reader.CurrentLineTrimmed}'. Only Given, And or But steps can be used in a Background.");
            }

            var stepTitle = reader.CurrentLineTrimmed;
            reader.ReadNextLine();

            var multiLineString = MultiLineStringParser.ParseMultiLineStringIfPresent(reader);

            var dataTable = DataTable.Empty;
            if (reader.IsTableLine)
            {
                dataTable = DataTableParser.ParseDataTable(reader);
            }

            return new GivenStep(
                stepTitle,
                dataTable,
                multiLineString);
        }
    }
}
