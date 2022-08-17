using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GherkinSpec.Model.Parsing
{
    public class Parser
    {
        public Feature Parse(string featureText)
        {
            using var reader = new StringReader(featureText);
            return Parse(reader);
        }

        public Feature Parse(TextReader reader)
            => Parse(new LineReader(reader));

        private Feature Parse(LineReader reader)
        {
            reader.ReadNextLine();

            var featureTags = TagParser.ParseTagsIfPresent(reader).ToArray();

            Localisation.SetUiCultureFromTag(featureTags);

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
                && !reader.IsRuleStartLine
                && !reader.IsScenarioStartLine
                && !reader.IsScenarioOutlineStartLine
                && !reader.IsBackgroundStartLine
                && !reader.IsTagLine)
            {
                featureNarrative.AppendLine(reader.CurrentLineTrimmed);
                reader.ReadNextLine();
            }

            if (reader.IsEndOfFile)
            {
                return new Feature(
                    featureTitle,
                    featureNarrative.ToString().Trim(),
                    Background.Empty,
                    Enumerable.Empty<Scenario>(),
                    Enumerable.Empty<ScenarioOutline>(),
                    Enumerable.Empty<Rule>(),
                    featureTags);
            }

            var featureBackground = BackgroundParser.ParseBackgroundIfPresent(reader);

            var featureScenarios = new List<Scenario>();
            var featureScenarioOutlines = new List<ScenarioOutline>();
            var featureRules = new List<Rule>();

            var featureContentParser = new FeatureContentParser(featureScenarios, featureScenarioOutlines, featureRules, reader);
            featureContentParser.ParseFeatureContent();

            return new Feature(
                featureTitle,
                featureNarrative.ToString().Trim(),
                featureBackground,
                featureScenarios,
                featureScenarioOutlines,
                featureRules,
                featureTags);
        }
    }
}
