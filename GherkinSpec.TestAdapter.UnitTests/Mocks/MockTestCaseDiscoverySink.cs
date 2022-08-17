using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    internal class MockTestCaseDiscoverySink : ITestCaseDiscoverySink
    {
        private readonly List<TestCase> discoveredTests = new();

        public IReadOnlyList<TestCase> DiscoveredTests
            => discoveredTests.AsReadOnly();

        public void SendTestCase(TestCase discoveredTest)
        {
            discoveredTests.Add(discoveredTest);
        }
    }
}
