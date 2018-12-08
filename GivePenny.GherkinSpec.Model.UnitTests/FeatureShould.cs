using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GivePenny.GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class FeatureShould
    {
        [TestMethod]
        public void MergeSourceScenariosAndScenariosExpandedFromOutlines()
        {
            var feature = new Feature(
                "Title",
                "Narrative",
                Background.Empty,
                new[]
                {
                    new Scenario(
                        "Scenario",
                        Enumerable.Empty<IStep>(),
                        1,
                        Enumerable.Empty<Tag>())
                },
                new[]
                {
                    new ScenarioOutline(
                        "Scenario Outline",
                        Enumerable.Empty<IStep>(),
                        new DataTable(
                            new[] {
                                new DataTableRow(
                                    new[]
                                    {
                                        new DataTableCell("Header")
                                    }),
                                new DataTableRow(
                                    new[]
                                    {
                                        new DataTableCell("Value")
                                    })
                            }),
                        2,
                        Enumerable.Empty<Tag>())
                },
                Enumerable.Empty<Tag>());

            var allScenarios = feature.AllScenarios.ToArray();

            Assert.AreEqual(2, allScenarios.Length);
        }
    }
}
