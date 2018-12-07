using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GivePenny.GherkinSpec.Model.Parsing
{
    public class Parser
    {
        public Feature Parse(string featureText)
        {
            using (var reader = new StringReader(featureText))
            {
                return Parse(reader);
            }
        }

        public Feature Parse(TextReader reader)
            => Parse(new LineReader(reader));

        private Feature Parse(LineReader reader)
        {
            reader.ReadNextLine();

            if (!reader.IsFeatureStartLine)
            {
                throw new InvalidGherkinSyntaxException(
                    $"Expected the first line of a feature file to start with 'Feature:'.", reader.CurrentLineNumber);
            }

            var featureTitle = reader.CurrentLineFeatureTitle;
            var featureNarrative = new StringBuilder();

            reader.ReadNextLine();
            while (
                !reader.IsEndOfFile
                && !reader.IsScenarioStartLine
                && !reader.IsScenarioOutlineStartLine
                && !reader.IsBackgroundStartLine)
            {
                featureNarrative.AppendLine(reader.CurrentLine);
                reader.ReadNextLine();
            }

            if (reader.IsEndOfFile)
            {
                return new Feature(
                    featureTitle,
                    featureNarrative.ToString()?.Trim(),
                    Background.Empty,
                    Enumerable.Empty<Scenario>(),
                    Enumerable.Empty<ScenarioOutline>());
            }

            var featureBackground = BackgroundParser.ParseBackgroundIfPresent(reader);

            var scenarios = new List<Scenario>();
            var scenarioOutlines = new List<ScenarioOutline>();
            do
            {
                if (reader.IsScenarioStartLine)
                {
                    scenarios.Add(ScenarioParser.ParseScenario(reader));
                    continue;
                }

                if (reader.IsScenarioOutlineStartLine)
                {
                    scenarioOutlines.Add(ScenarioParser.ParseScenarioOutline(reader));
                    continue;
                }

                throw new InvalidGherkinSyntaxException(
                    $"Expected a Scenario or a Scenario Outline.",
                    reader.CurrentLineNumber);

            } while (!reader.IsEndOfFile);

            return new Feature(
                featureTitle,
                featureNarrative.ToString()?.Trim(),
                featureBackground,
                scenarios,
                scenarioOutlines);
        }
    }
}
