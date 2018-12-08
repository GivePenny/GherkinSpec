using GherkinSpec.Model.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GherkinSpec.Model.UnitTests
{
    [TestClass]
    public class ParserShould
    {
        // TODO Ignore leading blank lines

        [TestMethod]
        public void ReadFeatureTitleAndIgnorePaddingWhitespace()
        {
            var text = "  Feature:  Adding things up ";
            var parser = new Parser();
            var feature = parser.Parse(text);
            Assert.AreEqual("Adding things up", feature.Title);
        }

        [TestMethod]
        public void ReadFeatureMotivationAfterFeatureTitleUntilScenarioLine()
        {
            var feature = ParseResource("FeatureMotivationScenarioTitle.feature");
            Assert.AreEqual(@"As a developer
I want things to work
So that I can have more fun", feature.Narrative);
        }

        [TestMethod]
        public void ReadScenarioTitle()
        {
            var feature = ParseResource("ScenarioTitle.feature");
            Assert.AreEqual(1, feature.Scenarios.Count());
            Assert.AreEqual(@"This is a scenario", feature.Scenarios.First().Title);
        }

        [TestMethod]
        public void ReadExampleAsAScenario()
        {
            var feature = ParseResource("Example.feature");
            Assert.AreEqual(1, feature.Scenarios.Count());
            Assert.AreEqual(@"This is a scenario", feature.Scenarios.First().Title);
        }

        [TestMethod]
        public void SkipCommentLines()
        {
            var feature = ParseResource("Comment.feature");
            Assert.IsTrue(string.IsNullOrEmpty(feature.Narrative));
        }

        [TestMethod]
        public void SkipWhitespaceLines()
        {
            var feature = ParseResource("Whitespace.feature");
            Assert.AreEqual("Motivation", feature.Narrative);
        }

        [TestMethod]
        public void ReadMultipleScenarioTitles()
        {
            var feature = ParseResource("ScenarioTitles.feature");

            Assert.AreEqual(3, feature.Scenarios.Count());
            Assert.AreEqual(@"This is a scenario", feature.Scenarios.First().Title);
            Assert.AreEqual(@"This is a 2nd scenario", feature.Scenarios.Second().Title);
            Assert.AreEqual(@"This is a 3rd scenario", feature.Scenarios.Third().Title);
        }

        [TestMethod]
        public void StoreScenarioStartingLineNumber()
        {
            var feature = ParseResource("ScenarioTitles.feature");

            Assert.AreEqual(2, feature.Scenarios.First().StartingLineNumber);
        }

        [TestMethod]
        public void ReadScenarioSteps()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            Assert.AreEqual(3, feature.Scenarios.Count());
            Assert.AreEqual(4, feature.Scenarios.First().Steps.Count);
            Assert.AreEqual(4, feature.Scenarios.Second().Steps.Count);
        }

        [TestMethod]
        public void ReadScenarioStepDataTableArguments()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            var secondScenario = feature.Scenarios.Second();
            Assert.AreEqual(2, secondScenario.Steps.First().TableArgument.Rows.Count);

            var secondTableRow = secondScenario.Steps.First().TableArgument.Rows.Second();
            Assert.AreEqual("value 2", secondTableRow.Cells.Second().Value);
        }

        [TestMethod]
        public void ReadScenarioStepMultiLineStringArguments()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            var thirdScenario = feature.Scenarios.Third();
            Assert.AreEqual(2, thirdScenario.Steps.Count());
            Assert.AreEqual(
                "A very long document can go here, but\r\n  watch that the indentation is correct.",
                thirdScenario.Steps.First().MultiLineStringArgument);
        }

        [TestMethod]
        public void ReadScenarioStepMultiLineStringAndDataTableArguments()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            var secondStep = feature.Scenarios.Third().Steps.Second();
            var secondRow = secondStep.TableArgument.Rows.Second();
            Assert.AreEqual(
                "Document here",
                secondStep.MultiLineStringArgument);
            Assert.AreEqual(
                "value 2",
                secondRow.Cells.Second().Value);
        }

        [TestMethod]
        public void TreatAndAsTheLastDefinedGivenWhenOrThen()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            Assert.IsInstanceOfType(
                feature.Scenarios.First().Steps.Second(),
                typeof(GivenStep));
        }

        [TestMethod]
        public void TreatButAsTheLastDefinedGivenWhenOrThen()
        {
            var feature = ParseResource("ScenarioSteps.feature");

            Assert.IsInstanceOfType(
                feature.Scenarios.Second().Steps.Third(),
                typeof(ThenStep));
        }

        [TestMethod]
        public void ReadFeatureBackground()
        {
            var feature = ParseResource("Background.feature");

            Assert.AreEqual(2, feature.Background.Steps.Count);
            Assert.AreEqual(@"Given a first step", feature.Background.Steps.First().Title);
            Assert.AreEqual(@"And another", feature.Background.Steps.Second().Title);
        }

        [TestMethod]
        public void ReadScenarioOutlineTitle()
        {
            var feature = ParseResource("ScenarioOutline.feature");

            Assert.AreEqual(1, feature.ScenarioOutlines.Count());
            Assert.AreEqual("Example scenario outline", feature.ScenarioOutlines.First().Title);
        }

        [TestMethod]
        public void ReadScenariosBeforeAndAfterOutlines()
        {
            var feature = ParseResource("ScenarioOutline.feature");

            Assert.AreEqual(2, feature.Scenarios.Count);
        }

        [TestMethod]
        public void ReadScenarioOutlineSteps()
        {
            var feature = ParseResource("ScenarioOutline.feature");

            var outlineSteps = feature.ScenarioOutlines.First().Steps;
            Assert.AreEqual(@"Given a first step <columnA>", outlineSteps.First().Title);
            Assert.AreEqual(@"And another <columnB>", outlineSteps.Second().Title);
        }

        [TestMethod]
        public void ReadScenarioOutlineExampleTable()
        {
            var feature = ParseResource("ScenarioOutline.feature");

            var exampleRows = feature.ScenarioOutlines.First().Examples.Rows;
            Assert.AreEqual(3, exampleRows.Count());
            Assert.AreEqual(2, exampleRows.First().Cells.Count());
            Assert.AreEqual(2, exampleRows.Second().Cells.Count());
        }

        [TestMethod]
        public void ReadScenarioOutlineExampleTableValues()
        {
            var feature = ParseResource("ScenarioOutline.feature");

            var exampleRows = feature.ScenarioOutlines.First().Examples.Rows;
            Assert.AreEqual("columnA", exampleRows.First().Cells.First().Value);
            Assert.AreEqual("columnB", exampleRows.First().Cells.Second().Value);
            Assert.AreEqual("A1", exampleRows.Second().Cells.First().Value);
            Assert.AreEqual("B1", exampleRows.Second().Cells.Second().Value);
            Assert.AreEqual("A2", exampleRows.Third().Cells.First().Value);
            Assert.AreEqual("B2", exampleRows.Third().Cells.Second().Value);
        }

        [TestMethod]
        public void ReadScenarioTagsAndPreserveCase()
        {
            var feature = ParseResource("Tags.feature");

            Assert.AreEqual(1, feature.Scenarios.Count());

            var firstScenario = feature.Scenarios.First();
            Assert.AreEqual(2, firstScenario.Tags.Count());
            Assert.AreEqual("ignore", firstScenario.Tags.First().Label);
            Assert.AreEqual("somethingElse", firstScenario.Tags.Second().Label);
        }

        [TestMethod]
        public void ReadScenarioOutlineTagsAndPreserveCase()
        {
            var feature = ParseResource("Tags.feature");
            Assert.AreEqual(1, feature.ScenarioOutlines.Count());

            var firstScenarioOutline = feature.ScenarioOutlines.First();
            Assert.AreEqual(2, firstScenarioOutline.Tags.Count());
            Assert.AreEqual("ignore", firstScenarioOutline.Tags.First().Label);
            Assert.AreEqual("somethingOther", firstScenarioOutline.Tags.Second().Label);
        }

        [TestMethod]
        public void ReadFeatureTagsAndPreserveCase()
        {
            var feature = ParseResource("Tags.feature");
            Assert.AreEqual(2, feature.Tags.Count());
            Assert.AreEqual("ignore", feature.Tags.First().Label);
            Assert.AreEqual("featureTag", feature.Tags.Second().Label);
        }

        private Feature ParseResource(string resourceName)
            => new Parser().Parse(
                Resources.GetString(resourceName));
    }
}
