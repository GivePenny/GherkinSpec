using GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class WhenGivenRulesParserShould
    {
        [TestMethod]
        public void ReadAFeatureContainingRules()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual(3, feature.Rules.Count());
        }

        [TestMethod]
        public void ReadRuleTitles()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual("Rule title 1", feature.Rules.First().Title);
            Assert.AreEqual("Rule title 2", feature.Rules.Second().Title);
            Assert.AreEqual("Rule title 3", feature.Rules.Third().Title);
        }

        [TestMethod]
        public void ReadRuleBackground()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual(1, feature.Rules.First().Background.Steps.Count());
            Assert.AreEqual(@"Given a background step", feature.Rules.First().Background.Steps.First().Title);
        }

        [TestMethod]
        public void ReadRuleScenarioAfterRuleBackground()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual(@"Scenario title 1", feature.Rules.First().Scenarios.First().Title);
        }

        [TestMethod]
        public void ReadRuleScenarioAfterRule()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual(@"Scenario title 2", feature.Rules.Second().Scenarios.First().Title);
        }

        [TestMethod]
        public void ReadRuleScenarioOutlineAfterRule()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual(@"Scenario title 3", feature.Rules.Third().ScenarioOutlines.First().Title);
        }

        [TestMethod]
        public void ReadRuleTags()
        {
            var feature = ParseResource("FeatureWithRules.feature");
            Assert.AreEqual("ignore", feature.Rules.Third().Tags.First().Label);
        }

        private Feature ParseResource(string resourceName)
            => new Parser().Parse(
                Resources.GetString(resourceName));
    }
}
