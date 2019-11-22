using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class Rule
    {
        public Rule(
            string title,
            Background background,
            IEnumerable<Scenario> scenarios,
            IEnumerable<ScenarioOutline> scenarioOutlines,
            IEnumerable<Tag> tags)
        {
            Title = title;
            Background = background;
            Scenarios = scenarios.ToList().AsReadOnly();
            ScenarioOutlines = new ReadOnlyScenarioOutlineCollection(
                scenarioOutlines.ToList());
            Tags = new ReadOnlyTagCollection(tags.ToList());
        }

        public string Title { get; }

        public Background Background { get; }

        public IReadOnlyCollection<Scenario> Scenarios { get; }

        public ReadOnlyScenarioOutlineCollection ScenarioOutlines { get; }

        public ReadOnlyTagCollection Tags { get; }

        public IEnumerable<Scenario> AllScenarios
            => Scenarios.Concat(ScenarioOutlines.ResultingScenarios());

        public bool IsIgnored
            => Tags.Any(
                tag => Localisation.IsLocalisedValue(
                    tag.Label,
                    Resources.IgnoreTagKeywords));
    }
}
