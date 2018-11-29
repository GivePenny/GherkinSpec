using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.IO;
using System.Linq;

namespace GivePenny.GherkinCore.TestAdapter
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

            // TODO Check for gherkin extension if resourceName ends with that instead of feature
            // TODO Stop runaway process scanning entire disk

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

                // Grab paths of all feature files
                foreach (var featureFile in Directory
                    .EnumerateFiles(folderName, "*.feature", SearchOption.AllDirectories))
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
