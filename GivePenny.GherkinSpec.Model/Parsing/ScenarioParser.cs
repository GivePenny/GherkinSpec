using System;
using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model.Parsing
{
    static class ScenarioParser
    {
        public static Scenario ParseScenario(LineReader reader)
        {
            var scenarioTitle = reader.CurrentLineScenarioTitle;
            var scenarioStartingLineNumber = reader.CurrentLineNumber;
            var steps = new List<IStep>();

            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                steps.Add(ParseStep(reader, steps.LastOrDefault()));
                reader.ReadNextLine();

                if (!reader.IsEndOfFile
                    && !reader.IsStepLine
                    && !reader.IsScenarioStartLine
                    && !reader.IsScenarioOutlineStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a step (Given, When, Then), a Scenario or a Scenario Outline, but found '{reader.CurrentLine}'.", reader.CurrentLineNumber);
                }
            }

            return new Scenario(scenarioTitle, steps, scenarioStartingLineNumber);
        }

        public static ScenarioOutline ParseScenarioOutline(LineReader reader)
        {
            var scenarioOutlineTitle = reader.CurrentLineScenarioOutlineTitle;
            var scenarioOutlineStartingLineNumber = reader.CurrentLineNumber;
            var steps = new List<IStep>();
            var examples = DataTable.Empty;

            var alreadyConsumedExamples = false;
            reader.ReadNextLine();
            while (reader.IsStepLine)
            {
                steps.Add(ParseStep(reader, steps.LastOrDefault()));
                reader.ReadNextLine();

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
                    && !reader.IsScenarioOutlineStartLine)
                {
                    throw new InvalidGherkinSyntaxException(
                        $"Expected a step (Given, When, Then), a Scenario or a Scenario Outline, but found '{reader.CurrentLine}'.",
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
                scenarioOutlineStartingLineNumber);
        }

        private static DataTable ParseExamples(LineReader reader)
        {
            reader.ReadNextLine();
            return ParseDataTable(reader);
        }

        private static DataTable ParseDataTable(LineReader reader)
        {
            var rows = new List<DataTableRow>();

            while (reader.IsTableLine)
            {
                rows.Add(ParseDataTableRow(reader));
                reader.ReadNextLine();
            };

            return new DataTable(rows);
        }

        private static DataTableRow ParseDataTableRow(LineReader reader)
        {
            var column = reader
                .CurrentLine
                .RemoveFirstAndLastPipes(
                    reader.CurrentLineNumber)
                .Split('|');

            var cells = new List<DataTableCell>();

            foreach (var value in column)
            {
                cells.Add(
                    new DataTableCell(
                        value.Trim()));
            }

            return new DataTableRow(cells);
        }

        private static IStep ParseStep(LineReader reader, IStep previousStep)
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
