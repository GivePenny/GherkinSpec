using GherkinSpec.Model.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class Scenario
    {
        private EventuallyConsistentScenarioConfiguration eventuallyConsistentConfiguration;

        public Scenario(
            string title,
            IEnumerable<IStep> steps,
            int startingLineNumber,
            IEnumerable<Tag> tags)
        {
            Title = title;
            StartingLineNumber = startingLineNumber;
            Steps = steps.ToList().AsReadOnly();
            Tags = new ReadOnlyTagCollection(tags.ToList());
        }

        public string Title { get; }
        public int StartingLineNumber { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
        public ReadOnlyTagCollection Tags { get; }

        public EventuallyConsistentScenarioConfiguration EventuallyConsistentConfiguration => eventuallyConsistentConfiguration;

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

        public bool IsEventuallyConsistent
            => Tags.Any(
                tag =>
                    EventuallyConsistentTagParser
                        .TryParse(
                            tag.Label,
                            out eventuallyConsistentConfiguration));
    }
}
