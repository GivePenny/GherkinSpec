using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class TestDiscovererShould
    {
        [TestMethod]
        public void GracefullyHandleDuplicateScenarioNames()
        {
            var testCaseDiscoverySink = new TestCaseDiscoverySink();
            new TestDiscoverer().DiscoverTests(
                new[]
                {
                    "GherkinSpec.TestAdapter.UnitTests.dll"
                },
                null,
                new NullMessageLogger(),
                testCaseDiscoverySink);

            Assert.AreEqual(
                1,
                testCaseDiscoverySink.DiscoveredTests.Count(
                    t => t.FullyQualifiedName == "Duplicate scenario"
                        && t.DisplayName == "Duplicate scenario"));

            Assert.AreEqual(
                1,
                testCaseDiscoverySink.DiscoveredTests.Count(
                    t => t.FullyQualifiedName == "Duplicate scenario (2)"
                        && t.DisplayName == "Duplicate scenario (2)"));

            Assert.AreEqual(
                1,
                testCaseDiscoverySink.DiscoveredTests.Count(
                    t => t.FullyQualifiedName == "Duplicate scenario (3)"
                        && t.DisplayName == "Duplicate scenario (3)"));
        }

        private class NullMessageLogger : IMessageLogger
        {
            public void SendMessage(TestMessageLevel testMessageLevel, string message)
            {
            }
        }

        private class TestCaseDiscoverySink : ITestCaseDiscoverySink
        {
            private readonly List<TestCase> discoveredTests = new List<TestCase>();

            public IReadOnlyList<TestCase> DiscoveredTests
                => discoveredTests.AsReadOnly();

            public void SendTestCase(TestCase discoveredTest)
            {
                discoveredTests.Add(discoveredTest);
            }
        }
    }
}
