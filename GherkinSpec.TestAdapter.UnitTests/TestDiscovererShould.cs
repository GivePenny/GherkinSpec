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
        public void GracefullyHandleDuplicateScenarioNamesInASource()
        {
            var testCaseDiscoverySink = new MockTestCaseDiscoverySink();
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
    }
}
