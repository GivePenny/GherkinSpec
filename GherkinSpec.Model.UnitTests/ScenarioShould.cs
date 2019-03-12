using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class ScenarioShould
    {
        [TestMethod]
        public void BeMarkedAsIgnoredWhenCreatedWithAnIgnoreTag()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("ignore")
            });

            Assert.IsTrue(scenario.IsIgnored);
        }

        [TestMethod]
        public void BeMarkedAsIgnoredWhenCreatedWithAnIgnoredTag()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("ignored")
            });

            Assert.IsTrue(scenario.IsIgnored);
        }

        [TestMethod]
        public void BeMarkedAsEventuallyConsistentWhenCreatedWithAnEventuallyConsistentTag()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("eventuallyConsistent")
            });

            Assert.IsTrue(scenario.IsEventuallyConsistent);
        }

        [TestMethod]
        public void BeMarkedAsEventuallyConsistentWhenCreatedWithAnEventuallyConsistentTagWithBrackets()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("eventuallyConsistent()")
            });

            Assert.IsTrue(scenario.IsEventuallyConsistent);
        }

        [TestMethod]
        public void BeMarkedAsEventuallyConsistentWhenCreatedWithAnEventuallyConsistentTagWithAWithinParameter()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("eventuallyConsistent(within=00:00:20)")
            });

            Assert.IsTrue(scenario.IsEventuallyConsistent);
        }

        [TestMethod]
        public void BeMarkedAsEventuallyConsistentWhenCreatedWithAnEventuallyConsistentTagWithARetryIntervalParameter()
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag("eventuallyConsistent(retryInterval=00:00:05)")
            });

            Assert.IsTrue(scenario.IsEventuallyConsistent);
        }

        [DataTestMethod]
        [DataRow("eventuallyConsistent(within=00:00:20,retryInterval=00:00:05)")]
        [DataRow("eventuallyConsistent(retryInterval=00:00:05,within=00:00:20)")]
        public void BeMarkedAsEventuallyConsistentWhenCreatedWithAnEventuallyConsistentTagWithAWithinParameterAndARetryIntervalParameter(string label)
        {
            var scenario = new Scenario("Title", Enumerable.Empty<IStep>(), 0, new List<Tag>
            {
                new Tag(label)
            });

            Assert.IsTrue(scenario.IsEventuallyConsistent);
        }
    }
}
