using System.Collections.Generic;

namespace GivePenny.GherkinSpec.Model
{
    public class Feature
    {
        public Feature(string title, string motivation, IEnumerable<Scenario> scenarios)
        {
            Title = title;
            Motivation = motivation;
            Scenarios = new List<Scenario>(scenarios).AsReadOnly();
        }

        public string Title { get; }
        public string Motivation { get; }

        public IReadOnlyCollection<Scenario> Scenarios { get; }
    }
}
