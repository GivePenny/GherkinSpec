using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class ScenarioOutline
    {
        public ScenarioOutline(
            string title,
            IEnumerable<IStep> steps,
            DataTable examples,
            int startingLineNumber,
            IEnumerable<Tag> tags)
        {
            Title = title;
            Examples = examples;
            StartingLineNumber = startingLineNumber;
            Steps = steps.ToList().AsReadOnly();
            Tags = tags.ToList().AsReadOnly();
        }

        public string Title { get; }
        public int StartingLineNumber { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
        public DataTable Examples { get; }
        public IReadOnlyCollection<Tag> Tags { get; }
    }
}
