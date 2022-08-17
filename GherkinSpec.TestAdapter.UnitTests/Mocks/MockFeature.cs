using GherkinSpec.Model;
using System.Linq;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    internal class MockFeature : Feature
    {
        public MockFeature() : base(
            "Test feature",
            null,
            Background.Empty,
            Enumerable.Empty<Scenario>(),
            Enumerable.Empty<ScenarioOutline>(),
            Enumerable.Empty<Rule>(),
            Enumerable.Empty<Tag>())
        {
        }
    }
}
