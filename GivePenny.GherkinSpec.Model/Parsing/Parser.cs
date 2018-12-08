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

            var featureTags = ParseTagsIfPresent(reader);

            if (!reader.IsFeatureStartLine)
            {
                throw new InvalidGherkinSyntaxException(
                    $"Expected the first line of a feature file to start with 'Feature:' or a tag.", reader.CurrentLineNumber);
            }

            var featureTitle = reader.CurrentLineFeatureTitle;
            var featureNarrative = new StringBuilder();

            reader.ReadNextLine();
            while (
                !reader.IsEndOfFile
                && !reader.IsScenarioStartLine
                && !reader.IsScenarioOutlineStartLine
                && !reader.IsBackgroundStartLine
                && !reader.IsTagLine)
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
                    Enumerable.Empty<ScenarioOutline>(),
                    featureTags);
            }

            var featureBackground = BackgroundParser.ParseBackgroundIfPresent(reader);

            var scenarios = new List<Scenario>();
            var scenarioOutlines = new List<ScenarioOutline>();

            do
            {
                var tags = ParseTagsIfPresent(reader);

                if (reader.IsScenarioStartLine)
                {
                    scenarios.Add(
                        ScenarioParser.ParseScenario(reader, tags));
                    continue;
                }

                if (reader.IsScenarioOutlineStartLine)
                {
                    scenarioOutlines.Add(
                        ScenarioParser.ParseScenarioOutline(reader, tags));
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
                scenarioOutlines,
                featureTags);
        }

        private IEnumerable<Tag> ParseTagsIfPresent(LineReader reader)
        {
            if (!reader.IsTagLine)
            {
                return Enumerable.Empty<Tag>();
            }

            var tags = new List<Tag>();

            while (reader.IsTagLine)
            {
                var tagsOnLine = reader.CurrentLine.Split(',');
                foreach (var tag in tagsOnLine)
                {
                    var cleanTag = tag.Trim();
                    if (!cleanTag.StartsWith("@"))
                    {
                        throw new InvalidGherkinSyntaxException(
                            "All tags must start with @.",
                            reader.CurrentLineNumber);
                    }

                    cleanTag = cleanTag
                        .Substring(1)
                        .TrimStart();

                    tags.Add(new Tag(cleanTag));
                }

                reader.ReadNextLine();
            }

            return tags;
        }
    }
}
