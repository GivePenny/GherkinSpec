using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.TestAdapter
{
    internal class TestSourceFile
    {
        public TestSourceFile(string sourceFileName, IEnumerable<string> relevantFolderNames)
        {
            SourceFileName = sourceFileName;
            RelevantFolderNames = relevantFolderNames.ToList().AsReadOnly();
        }

        public string SourceFileName { get; }
        public IReadOnlyCollection<string> RelevantFolderNames { get; }
    }
}
