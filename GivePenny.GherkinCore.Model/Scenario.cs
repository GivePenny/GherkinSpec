using System.Collections.Generic;

namespace GivePenny.GherkinCore.Model
{
    public class Scenario
    {
        public Scenario(string title, IEnumerable<IStep> steps, int startingLineNumber)
        {
            Title = title;
            StartingLineNumber = startingLineNumber;
            Steps = new List<IStep>(steps).AsReadOnly();
        }

        public string Title { get; }
        public int StartingLineNumber { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
    }
}
