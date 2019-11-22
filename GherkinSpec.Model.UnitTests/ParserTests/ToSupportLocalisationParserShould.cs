using GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Linq;

namespace GherkinSpec.Model.UnitTests.ParserTests
{
    [TestClass]
    public class ToSupportLocalisationParserShould
    {
        [TestMethod]
        public void ReadFeatureUsingTaggedCulture()
        {
            var feature = ParseResource("Localisation.Feature.feature");
            Assert.AreEqual("Localised feature title", feature.Title);
            Assert.AreEqual("nb-NO", CultureInfo.CurrentUICulture.Name);
        }

        [TestMethod]
        public void DefaultToInvariantCulture()
        {
            var feature = ParseResource("Localisation.Default culture.feature");
            Assert.AreEqual("Localised feature title", feature.Title);
            Assert.AreEqual(string.Empty, CultureInfo.CurrentUICulture.Name);
        }

        [TestMethod]
        public void ReadLocalisedScenarios()
        {
            var feature = ParseResource("Localisation.Feature.feature");

            Assert.AreEqual(1, feature.Scenarios.Count());
        }

        [TestMethod]
        public void ReadLocalisedScenarioStepTypes()
        {
            var feature = ParseResource("Localisation.Feature.feature");

            var steps = feature.Scenarios.First().Steps;
            Assert.AreEqual(5, steps.Count);
            Assert.IsInstanceOfType(steps.First(), typeof(GivenStep));
            Assert.IsInstanceOfType(steps.Second(), typeof(WhenStep));
            Assert.IsInstanceOfType(steps.Third(), typeof(ThenStep));
            Assert.IsInstanceOfType(steps.Fourth(), typeof(ThenStep));
            Assert.IsInstanceOfType(steps.Fifth(), typeof(ThenStep));
        }

        [TestMethod]
        public void ParseLocalisedScenarioStepTitles()
        {
            var feature = ParseResource("Localisation.Feature.feature");

            var steps = feature.Scenarios.First().Steps;
            Assert.AreEqual("given", steps.First().TitleAfterType);
            Assert.AreEqual("when", steps.Second().TitleAfterType);
            Assert.AreEqual("then", steps.Third().TitleAfterType);
            Assert.AreEqual("and", steps.Fourth().TitleAfterType);
            Assert.AreEqual("but", steps.Fifth().TitleAfterType);
        }

        private Feature ParseResource(string resourceName)
            => new Parser().Parse(
                Resources.GetString(resourceName));
    }
}
