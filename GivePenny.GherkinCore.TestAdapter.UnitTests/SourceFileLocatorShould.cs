using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Reflection;

namespace GivePenny.GherkinCore.TestAdapter.UnitTests
{
    [TestClass]
    public class SourceFileLocatorShould
    {
        private readonly Mock<IMessageLogger> mockLogger = new Mock<IMessageLogger>();

        [TestMethod]
        public void FindFeatureFile()
        {
            var locator = new SourceFileLocator(
                assemblyDllPath: Assembly.GetExecutingAssembly().Location);

            var resourceNameToFind = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(name => name.EndsWith("FindMe.feature"))
                .Single();

            var foundFile = locator.FindFeatureFileNameIfPossible(
                resourceNameToFind,
                mockLogger.Object);

            Assert.IsNotNull(foundFile);
            foundFile = foundFile.Replace("\\", "/");

            Assert.IsTrue(foundFile.EndsWith("GivePenny.GherkinCore.TestAdapter.UnitTests/FeaturesToFind/FindMe.feature"));
        }
    }
}
