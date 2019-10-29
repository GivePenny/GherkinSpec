using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter
{
    class UniqueTestCaseNameResolver
    {
        private static readonly Regex uniqueTestCaseSuffixRegex = new Regex(" \\((\\d+)\\)$");
        private readonly HashSet<string> discoveredFullyQualifiedTestNames = new HashSet<string>();

        public void EnsureTestCaseNameIsUnique(TestCase testCase)
        {
            if (!discoveredFullyQualifiedTestNames.Contains(testCase.FullyQualifiedName))
            {
                discoveredFullyQualifiedTestNames.Add(testCase.FullyQualifiedName);
                return;
            }

            var distinctTestCaseSuffixMatch = uniqueTestCaseSuffixRegex.Match(testCase.FullyQualifiedName);
            if (!distinctTestCaseSuffixMatch.Success)
            {
                testCase.FullyQualifiedName += " (2)";
                testCase.DisplayName += " (2)";
            }
            else
            {
                var nextDistinctTestCaseCounterValue = int.Parse(distinctTestCaseSuffixMatch.Groups[1].Value) + 1;

                testCase.FullyQualifiedName = uniqueTestCaseSuffixRegex.Replace(
                    testCase.FullyQualifiedName,
                    $" ({nextDistinctTestCaseCounterValue})");

                testCase.DisplayName = uniqueTestCaseSuffixRegex.Replace(
                    testCase.DisplayName,
                    $" ({nextDistinctTestCaseCounterValue})");
            }

            EnsureTestCaseNameIsUnique(testCase);
            return;
        }
    }
}
