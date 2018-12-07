using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class ScenarioOutline
    {
        public ScenarioOutline(string title, IEnumerable<IStep> steps, DataTable examples, int startingLineNumber)
        {
            Title = title;
            Examples = examples;
            StartingLineNumber = startingLineNumber;
            Steps = steps.ToList().AsReadOnly();
        }

        public string Title { get; }
        public int StartingLineNumber { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
        public DataTable Examples { get; }
    }
}
