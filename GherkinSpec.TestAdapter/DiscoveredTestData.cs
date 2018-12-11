using GherkinSpec.Model;
using System.Reflection;

namespace GherkinSpec.TestAdapter
{
    class DiscoveredTestData
    {
        public DiscoveredTestData(Assembly assembly, Feature feature, Scenario scenario)
        {
            Assembly = assembly;
            Feature = feature;
            Scenario = scenario;
        }

        public Assembly Assembly { get; }
        public Feature Feature { get; }
        public Scenario Scenario { get; }

        public bool IsIgnored
            => Feature.IsIgnored || Scenario.IsIgnored;
    }
}
