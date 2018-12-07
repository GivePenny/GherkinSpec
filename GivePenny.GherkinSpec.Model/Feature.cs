using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class Feature
    {
        public Feature(
            string title,
            string narrative,
            Background background,
            IEnumerable<Scenario> scenarios,
            IEnumerable<ScenarioOutline> scenarioOutlines)
        {
            Title = title;
            Narrative = narrative;
            Background = background;
            Scenarios = scenarios.ToList().AsReadOnly();
            ScenarioOutlines = new ReadOnlyScenarioOutlineCollection(
                scenarioOutlines.ToList());
        }

        public string Title { get; }
        public string Narrative { get; }
        public Background Background { get; }
        public IReadOnlyCollection<Scenario> Scenarios { get; }
        public ReadOnlyScenarioOutlineCollection ScenarioOutlines { get; }

        public IEnumerable<Scenario> AllScenarios
            => Scenarios.Concat(ScenarioOutlines.ResultingScenarios());
    }
}
