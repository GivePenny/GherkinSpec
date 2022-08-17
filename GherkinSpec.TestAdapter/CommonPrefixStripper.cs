using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;

namespace GherkinSpec.TestAdapter
{
    internal static class CommonPrefixStripper
    {
        public static void StripNamePrefixesSharedByAllTestCases(List<TestCase> cases)
        {
            if (cases.Count == 0)
            {
                return;
            }

            var candidatePrefix = (string)null;

            foreach (var testCase in cases)
            {
                var indexOfPeriod = testCase.FullyQualifiedName.IndexOf('.');
                if (indexOfPeriod == -1)
                {
                    // Some tests have no prefix at all, so nothing can be stripped from this round.
                    return;
                }

                var currentPrefix = testCase.FullyQualifiedName.Substring(0, indexOfPeriod + 1);

                if (candidatePrefix == null)
                {
                    candidatePrefix = currentPrefix;
                    continue;
                }

                if (candidatePrefix != currentPrefix)
                {
                    // Found a test with a non-matching prefix, nothing can be stripped from this round.
                    return;
                }
            }

            foreach (var testCase in cases)
            {
                testCase.FullyQualifiedName = testCase.FullyQualifiedName.Substring(candidatePrefix.Length);
            }

            StripNamePrefixesSharedByAllTestCases(cases);
        }
    }
}
