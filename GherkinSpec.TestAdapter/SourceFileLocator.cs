using System;
using System.IO;
using System.Linq;

namespace GherkinSpec.TestAdapter
{
    internal class SourceFileLocator
    {
        private readonly string assemblyDllPath;

        public SourceFileLocator(string assemblyDllPath)
        {
            this.assemblyDllPath = assemblyDllPath;
        }

        public TestSourceFile FindFeatureFileNameIfPossible(string resourceName)
        {
            var folderName = assemblyDllPath;

            var filePatternToFind = "*.feature";
            if (resourceName.EndsWith(
                ".gherkin",
                StringComparison.OrdinalIgnoreCase))
            {
                filePatternToFind = "*.gherkin";
            }

            do
            {
                folderName = Path.GetDirectoryName(folderName);
                if (string.IsNullOrEmpty(folderName))
                {
                    return new TestSourceFile(
                        null,
                        Enumerable.Empty<string>());
                }

                var containsProject = Directory
                    .EnumerateFiles(folderName, "*.csproj")
                    .Any();

                if (!containsProject)
                {
                    continue;
                }

                foreach (var featureFile in
                    Directory.EnumerateFiles(folderName, filePatternToFind, SearchOption.AllDirectories))
                {
                    var featureFileName = Path.GetFileName(featureFile);
                    var featureFileFolder = Path.GetDirectoryName(featureFile)
                        .Substring(folderName.Length);

                    var possibleResourcifiedName = featureFileFolder
                        .Replace(' ', '_')
                        + '.' + featureFileName;

                    possibleResourcifiedName = possibleResourcifiedName
                        .Replace('/', '.')
                        .Replace('\\', '.');

                    if (resourceName.EndsWith(possibleResourcifiedName))
                    {
                        return new TestSourceFile(
                            featureFile,
                            featureFileFolder.Split(
                                new[] { Path.DirectorySeparatorChar },
                                StringSplitOptions.RemoveEmptyEntries));
                    }
                }

            } while (true);
        }
    }
}
