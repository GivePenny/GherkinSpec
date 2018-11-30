using System.Collections.Generic;

namespace GivePenny.GherkinSpec.Model
{
    public class Feature
    {
        public Feature(string title, string narrative, IEnumerable<Scenario> scenarios)
        {
            Title = title;
            Narrative = narrative;
            Scenarios = new List<Scenario>(scenarios).AsReadOnly();
        }

        public string Title { get; }
        public string Narrative { get; }

        public IReadOnlyCollection<Scenario> Scenarios { get; }
    }
}
