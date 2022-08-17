using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;

namespace GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class SourceFileLocatorShould
    {
        [TestMethod]
        public void FindFeatureFileWithFeatureExtension()
            => FindFeatureFileWithExtension("feature");

        [TestMethod]
        public void FindFeatureFileWithGherkinExtension()
            => FindFeatureFileWithExtension("gherkin");

        private void FindFeatureFileWithExtension(string extension)
        {
            var foundFile = FindFeatureFile($"FindMe.{extension}");

            var foundFileSourceName = foundFile.SourceFileName;
            Assert.IsNotNull(foundFileSourceName);
            foundFileSourceName = foundFileSourceName.Replace("\\", "/");

            Assert.IsTrue(
                foundFileSourceName.EndsWith(
                    $"GherkinSpec.TestAdapter.UnitTests/FeaturesToFind/FindMe.{extension}"));
        }

        private TestSourceFile FindFeatureFile(string resourceNameEnding)
        {
            var locator = new SourceFileLocator(
                assemblyDllPath: Assembly.GetExecutingAssembly().Location);

            var resourceNameToFind = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(name => name.EndsWith(resourceNameEnding))
                .Single();

            return locator.FindFeatureFileNameIfPossible(
                resourceNameToFind);
        }

        [TestMethod]
        public void FindFeatureFileInFolder()
        {
            var foundFile = FindFeatureFile("Find me too.feature");

            var foundFileSourceName = foundFile.SourceFileName;
            Assert.IsNotNull(foundFileSourceName);
            foundFileSourceName = foundFileSourceName.Replace("\\", "/");

            Assert.IsTrue(
                foundFileSourceName.EndsWith(
                    $"GherkinSpec.TestAdapter.UnitTests/FeaturesToFind/In a folder/Find me too.feature"));
        }

        [TestMethod]
        public void IdentifyFolderNameComponents()
        {
            var foundFile = FindFeatureFile("Find me too.feature");
            var folderNames = foundFile.RelevantFolderNames;
            Assert.AreEqual(2, folderNames.Count);
            Assert.AreEqual("FeaturesToFind", folderNames.First());
            Assert.AreEqual("In a folder", folderNames.Skip(1).First());
        }
    }
}
