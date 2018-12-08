using System;
using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class Scenario
    {
        public Scenario(
            string title,
            IEnumerable<IStep> steps,
            int startingLineNumber,
            IEnumerable<Tag> tags)
        {
            Title = title;
            StartingLineNumber = startingLineNumber;
            Steps = steps.ToList().AsReadOnly();
            Tags = tags.ToList().AsReadOnly();
        }

        public string Title { get; }
        public int StartingLineNumber { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
        public IReadOnlyCollection<Tag> Tags { get; }

        public bool IsIgnored
            => Tags.Any(
                tag => string.Equals(
                    tag.Label,
                    "ignored",
                    StringComparison.OrdinalIgnoreCase));
    }
}
