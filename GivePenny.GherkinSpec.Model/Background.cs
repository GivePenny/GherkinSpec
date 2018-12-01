using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class Background
    {
        public Background(IEnumerable<GivenStep> steps, int startingLineNumber)
        {
            StartingLineNumber = startingLineNumber;
            Steps = new List<GivenStep>(steps).AsReadOnly();
        }

        public int StartingLineNumber { get; }
        public IReadOnlyCollection<GivenStep> Steps { get; }

        public static Background Empty
            => new Background(
                Enumerable.Empty<GivenStep>(), 0);
    }
}
