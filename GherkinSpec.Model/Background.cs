using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class Background
    {
        public Background(IEnumerable<GivenStep> steps, int startingLineNumber)
        {
            StartingLineNumber = startingLineNumber;
            Steps = steps.ToList().AsReadOnly();
        }

        public int StartingLineNumber { get; }
        public IReadOnlyCollection<GivenStep> Steps { get; }

        public static Background Empty
            => new(
                Enumerable.Empty<GivenStep>(), 0);
    }
}
