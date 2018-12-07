using GivePenny.GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GivePenny.GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class ReadOnlyScenarioOutlineCollectionShould
    {
        [TestMethod]
        public void ExpandScenarioOutlinesIntoScenarios()
        {
            var text = Resources.GetString("ScenarioOutline.feature");
            var parser = new Parser();
            var feature = parser.Parse(text);

            Assert.AreEqual(2, feature.ScenarioOutlines.ResultingScenarios().Count());
        }

        [TestMethod]
        public void PopulateTitle_WhenExpandingScenarioOutlinesIntoScenarios()
        {
            var text = Resources.GetString("ScenarioOutline.feature");
            var parser = new Parser();
            var feature = parser.Parse(text);

            var firstScenario = feature.ScenarioOutlines.ResultingScenarios().First();
            Assert.AreEqual("Example scenario outline (A1, B1)", firstScenario.Title);
        }

        [TestMethod]
        public void PopulateSteps_WhenExpandingScenarioOutlinesIntoScenarios()
        {
            var text = Resources.GetString("ScenarioOutline.feature");
            var parser = new Parser();
            var feature = parser.Parse(text);

            var firstScenario = feature.ScenarioOutlines.ResultingScenarios().First();
            var secondScenario = feature.ScenarioOutlines.ResultingScenarios().Second();

            Assert.AreEqual(2, firstScenario.Steps.Count());
            Assert.AreEqual("Given a first step A1", firstScenario.Steps.First().Title);
            Assert.AreEqual("And another B1", firstScenario.Steps.Second().Title);
            Assert.AreEqual("Given a first step A2", secondScenario.Steps.First().Title);
            Assert.AreEqual("And another B2", secondScenario.Steps.Second().Title);
        }
    }
}
