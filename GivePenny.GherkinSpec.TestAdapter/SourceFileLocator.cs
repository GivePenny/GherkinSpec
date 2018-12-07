using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.IO;
using System.Linq;

namespace GivePenny.GherkinSpec.TestAdapter
{
    class SourceFileLocator
    {
        private readonly string assemblyDllPath;

        public SourceFileLocator(string assemblyDllPath)
        {
            this.assemblyDllPath = assemblyDllPath;
        }

        public string FindFeatureFileNameIfPossible(string resourceName, IMessageLogger logger)
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
                    return null;
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
                    var possibleResourcifiedName = featureFile
                        .Substring(folderName.Length)
                        .Replace("/", ".")
                        .Replace("\\", ".");

                    if (resourceName.EndsWith(possibleResourcifiedName))
                    {
                        return featureFile;
                    }
                }

            } while (true);
        }
    }
}
