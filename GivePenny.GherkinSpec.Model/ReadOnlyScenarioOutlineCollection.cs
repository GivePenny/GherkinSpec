using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class ReadOnlyScenarioOutlineCollection : ReadOnlyCollection<ScenarioOutline>
    {
        public ReadOnlyScenarioOutlineCollection(IList<ScenarioOutline> list)
            : base(list)
        {
        }

        public IEnumerable<Scenario> ResultingScenarios()
        {
            foreach (var scenarioOutline in this)
            {
                foreach (var row in scenarioOutline
                    .Examples
                    .Rows
                    .Skip(1))
                {
                    yield return CreateScenario(scenarioOutline, row);
                }
            }
        }

        private static Scenario CreateScenario(ScenarioOutline scenarioOutline, DataTableRow row)
        {
            var scenarioSteps = new List<IStep>();
            var scenarioTitle = $"{scenarioOutline.Title} ({row.Cells.CommaDelimitedValues})";
            var dataTable = scenarioOutline.Examples;

            foreach (var outlineStep in scenarioOutline.Steps)
            {
                var newTitle = dataTable.ReplacePlaceholdersWithValues(outlineStep.Title, row);
                var newTable = dataTable.ReplacePlaceholdersWithValues(outlineStep.TableArgument, row);
                var newMultiLineString = dataTable.ReplacePlaceholdersWithValues(outlineStep.MultiLineStringArgument, row);

                scenarioSteps.Add(
                    outlineStep.CreateAnother(
                        newTitle, newTable, newMultiLineString));
            }

            return new Scenario(
                scenarioTitle,
                scenarioSteps,
                scenarioOutline.StartingLineNumber,
                scenarioOutline.Tags);
        }
    }
}
