using System;
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
                    Enumerable.Empty<Scenario>());
            }

            var featureBackground = BackgroundParser.ParseBackgroundIfPresent(reader);

            var scenarios = new List<Scenario>();
            do
            {
                if (!reader.IsScenarioStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a Scenario.", reader.CurrentLineNumber);
                }

                scenarios.Add(ParseScenario(reader));
            } while (!reader.IsEndOfFile);

            return new Feature(
                featureTitle,
                featureNarrative.ToString()?.Trim(),
                featureBackground,
                scenarios);
        }

        private Scenario ParseScenario(LineReader reader)
        {
            var scenarioTitle = reader.CurrentLineScenarioTitle;
            var scenarioStartingLineNumber = reader.CurrentLineNumber;
            var steps = new List<IStep>();

            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                steps.Add(ParseStep(reader, steps.LastOrDefault()));
                reader.ReadNextLine();

                if (!reader.IsEndOfFile && !reader.IsStepLine && !reader.IsScenarioStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a step (Given, When, Then) or a Scenario, but found '{reader.CurrentLine}'.", reader.CurrentLineNumber);
                }
            }

            return new Scenario(scenarioTitle, steps, scenarioStartingLineNumber);
        }

        private IStep ParseStep(LineReader reader, IStep previousStep)
        {
            if (reader.IsGivenLine)
            {
                return new GivenStep(reader.CurrentLine);
            }

            if (reader.IsWhenLine)
            {
                return new WhenStep(reader.CurrentLine);
            }

            if (reader.IsThenLine)
            {
                return new ThenStep(reader.CurrentLine);
            }

            if (!reader.IsAndLine && !reader.IsButLine)
            {
                throw new NotSupportedException(
                    $"Unrecognised step type in line '{reader.CurrentLine}'.");
            }

            if (previousStep == null)
            {
                throw new InvalidGherkinSyntaxException(
                    $"The first step in a scenario cannot start with 'And'. Expected Given, When or Then", reader.CurrentLineNumber);
            }

            return previousStep.CreateAnother(reader.CurrentLine);
        }
    }
}
