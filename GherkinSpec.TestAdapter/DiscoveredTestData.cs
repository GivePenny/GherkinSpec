using GherkinSpec.Model;
using System.Reflection;

namespace GherkinSpec.TestAdapter
{
    public class DiscoveredTestData
    {
        public DiscoveredTestData(Assembly assembly, Feature feature, Rule rule, Scenario scenario)
        {
            Assembly = assembly;
            Feature = feature;
            Rule = rule;
            Scenario = scenario;
        }

        public Assembly Assembly { get; }

        public Feature Feature { get; }

        public Rule Rule { get; }

        public Scenario Scenario { get; }

        public bool IsIgnored
            => Feature.IsIgnored
                || Scenario.IsIgnored
                || (Rule?.IsIgnored).GetValueOrDefault(false);
    }
}
