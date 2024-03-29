﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model.Parsing
{
    internal static class ScenarioParser
    {
        public static Scenario ParseScenario(LineReader reader, IEnumerable<Tag> tags)
        {
            var scenarioTitle = reader.CurrentLineScenarioTitle;
            var scenarioStartingLineNumber = reader.CurrentLineNumber;
            var steps = new List<IStep>();

            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                steps.Add(
                    ParseStep(reader, steps.LastOrDefault()));

                if (!reader.IsEndOfFile
                    && !reader.IsStepLine
                    && !reader.IsScenarioStartLine
                    && !reader.IsScenarioOutlineStartLine
                    && !reader.IsTagLine
                    && !reader.IsRuleStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a step (Given, When, Then, And, But), a Scenario, a Scenario Outline, a Rule or a tag, but found '{reader.CurrentLineTrimmed}'.",
                        reader.CurrentLineNumber);
                }
            }

            return new Scenario(scenarioTitle, steps, scenarioStartingLineNumber, tags);
        }

        public static ScenarioOutline ParseScenarioOutline(LineReader reader, IEnumerable<Tag> tags)
        {
            var scenarioOutlineTitle = reader.CurrentLineScenarioOutlineTitle;
            var scenarioOutlineStartingLineNumber = reader.CurrentLineNumber;
            var steps = new List<IStep>();
            var examples = DataTable.Empty;

            var alreadyConsumedExamples = false;
            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                steps.Add(
                    ParseStep(reader, steps.LastOrDefault()));

                if (reader.IsExamplesStartLine)
                {
                    if (alreadyConsumedExamples)
                    {
                        throw new InvalidGherkinSyntaxException(
                            "A Scenario Outline can only contain one Examples section.",
                            reader.CurrentLineNumber);
                    }

                    examples = ParseExamples(reader);
                    alreadyConsumedExamples = true;
                }

                if (!reader.IsEndOfFile
                    && !reader.IsStepLine
                    && !reader.IsScenarioStartLine
                    && !reader.IsScenarioOutlineStartLine
                    && !reader.IsTagLine
                    && !reader.IsRuleStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a step (Given, When, Then, And, But), a Scenario, a Scenario Outline, a Rule or a tag, but found '{reader.CurrentLineTrimmed}'.",
                        reader.CurrentLineNumber);
                }
            }

            if (examples.Rows.Count < 2)
            {
                throw new InvalidGherkinSyntaxException(
                    $"Scenario Outline \"{scenarioOutlineTitle}\" is missing an Examples table, or the Examples table does not contain a column-header row and at least one data row.",
                    reader.CurrentLineNumber);
            }

            return new ScenarioOutline(
                scenarioOutlineTitle,
                steps,
                examples,
                scenarioOutlineStartingLineNumber,
                tags);
        }

        private static DataTable ParseExamples(LineReader reader)
        {
            reader.ReadNextLine();
            return DataTableParser.ParseDataTable(reader);
        }

        private static IStep ParseStep(LineReader reader, IStep previousStep)
        {
            var stepTitle = reader.CurrentLineTrimmed;

            if (reader.IsGivenLine)
            {
                return ParseStep(reader, (table, multiLineString) => new GivenStep(stepTitle, table, multiLineString));
            }

            if (reader.IsWhenLine)
            {
                return ParseStep(reader, (table, multiLineString) => new WhenStep(stepTitle, table, multiLineString));
            }

            if (reader.IsThenLine)
            {
                return ParseStep(reader, (table, multiLineString) => new ThenStep(stepTitle, table, multiLineString));
            }

            if (!reader.IsAndLine && !reader.IsButLine)
            {
                throw new NotSupportedException(
                    $"Unrecognised step type in line '{reader.CurrentLineTrimmed}'.");
            }

            if (previousStep == null)
            {
                throw new InvalidGherkinSyntaxException(
                    $"The first step in a scenario cannot start with 'And'. Expected Given, When or Then", reader.CurrentLineNumber);
            }

            return ParseStep(reader, (table, multiLineString) => previousStep.CreateAnother(stepTitle, table, multiLineString));
        }

        private static IStep ParseStep(LineReader reader, Func<DataTable, string, IStep> stepFactory)
        {
            reader.ReadNextLine();

            var multiLineString = MultiLineStringParser.ParseMultiLineStringIfPresent(reader);

            var dataTable = DataTable.Empty;
            if (reader.IsTableLine)
            {
                dataTable = DataTableParser.ParseDataTable(reader);
            }

            return stepFactory(dataTable, multiLineString);
        }
    }
}
