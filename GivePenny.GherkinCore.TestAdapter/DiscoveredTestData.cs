using GivePenny.GherkinCore.Model;
using System.Reflection;

namespace GivePenny.GherkinCore.TestAdapter
{
    class DiscoveredTestData
    {
        public DiscoveredTestData(Assembly assembly, Scenario scenario)
        {
            Assembly = assembly;
            Scenario = scenario;
        }

        public Assembly Assembly { get; }
        public Scenario Scenario { get; }
    }
}
