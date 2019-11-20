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

        private Feature ParseResource(string resourceName)
            => new Parser().Parse(
                Resources.GetString(resourceName));
    }
}
