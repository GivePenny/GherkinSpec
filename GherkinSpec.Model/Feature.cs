﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class Feature
    {
        public Feature(
            string title,
            string narrative,
            Background background,
            IEnumerable<Scenario> scenarios,
            IEnumerable<ScenarioOutline> scenarioOutlines,
            IEnumerable<Rule> rules,
            IEnumerable<Tag> tags)
        {
            Title = title;
            Narrative = narrative;
            Background = background;
            Scenarios = scenarios.ToList().AsReadOnly();
            ScenarioOutlines = new ReadOnlyScenarioOutlineCollection(
                scenarioOutlines.ToList());
            Rules = rules.ToList().AsReadOnly();
            Tags = tags.ToList().AsReadOnly();
        }

        public string Title { get; }
        public string Narrative { get; }
        public Background Background { get; }
        public IReadOnlyCollection<Scenario> Scenarios { get; }
        public ReadOnlyScenarioOutlineCollection ScenarioOutlines { get; }
        public IReadOnlyCollection<Rule> Rules { get; }
        public IReadOnlyCollection<Tag> Tags { get; }

        public IEnumerable<Scenario> AllScenarios
            => Scenarios.Concat(ScenarioOutlines.ResultingScenarios());

        public bool IsIgnored
            => Tags.Any(
                tag =>
                    string.Equals(
                        tag.Label,
                        "ignored",
                        StringComparison.OrdinalIgnoreCase)
                    || string.Equals(
                        tag.Label,
                        "ignore",
                        StringComparison.OrdinalIgnoreCase));
    }
}
