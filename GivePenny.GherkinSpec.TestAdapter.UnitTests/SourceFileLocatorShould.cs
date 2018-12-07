using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Reflection;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class SourceFileLocatorShould
    {
        private readonly Mock<IMessageLogger> mockLogger = new Mock<IMessageLogger>();

        [TestMethod]
        public void FindFeatureFileWithFeatureExtension()
            => FindFeatureFile("feature");

        [TestMethod]
        public void FindFeatureFileWithGherkinExtension()
            => FindFeatureFile("gherkin");

        private void FindFeatureFile(string extension)
        {
            var locator = new SourceFileLocator(
                assemblyDllPath: Assembly.GetExecutingAssembly().Location);

            var resourceNameToFind = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(name => name.EndsWith($"FindMe.{extension}"))
                .Single();

            var foundFile = locator.FindFeatureFileNameIfPossible(
                resourceNameToFind,
                mockLogger.Object);

            Assert.IsNotNull(foundFile);
            foundFile = foundFile.Replace("\\", "/");

            Assert.IsTrue(
                foundFile.EndsWith(
                    $"GivePenny.GherkinSpec.TestAdapter.UnitTests/FeaturesToFind/FindMe.{extension}"));
        }
    }
}
