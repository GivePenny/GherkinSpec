using System.Collections.Generic;

namespace GivePenny.GherkinSpec.Model
{
    public class Feature
    {
        public Feature(
            string title,
            string narrative,
            Background background,
            IEnumerable<Scenario> scenarios)
        {
            Title = title;
            Narrative = narrative;
            Background = background;
            Scenarios = new List<Scenario>(scenarios).AsReadOnly();
        }

        public string Title { get; }
        public string Narrative { get; }
        public Background Background { get; }
        public IReadOnlyCollection<Scenario> Scenarios { get; }
    }
}
