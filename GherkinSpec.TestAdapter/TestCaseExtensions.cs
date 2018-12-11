using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace GherkinSpec.TestAdapter
{
    static class TestCaseExtensions
    {
        public static DiscoveredTestData DiscoveredData(this TestCase testCase)
            => (DiscoveredTestData)testCase.LocalExtensionData;
    }
}
