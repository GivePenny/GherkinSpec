using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter
{
    internal class UniqueTestCaseNameResolver
    {
        private static readonly Regex UniqueTestCaseSuffixRegex = new(" \\((\\d+)\\)$");
        private readonly HashSet<string> discoveredFullyQualifiedTestNames = new();

        public void EnsureTestCaseNameIsUnique(TestCase testCase)
        {
            if (!discoveredFullyQualifiedTestNames.Contains(testCase.FullyQualifiedName))
            {
                discoveredFullyQualifiedTestNames.Add(testCase.FullyQualifiedName);
                return;
            }

            var distinctTestCaseSuffixMatch = UniqueTestCaseSuffixRegex.Match(testCase.FullyQualifiedName);
            if (!distinctTestCaseSuffixMatch.Success)
            {
                testCase.FullyQualifiedName += " (2)";
                testCase.DisplayName += " (2)";
            }
            else
            {
                var nextDistinctTestCaseCounterValue = int.Parse(distinctTestCaseSuffixMatch.Groups[1].Value) + 1;

                testCase.FullyQualifiedName = UniqueTestCaseSuffixRegex.Replace(
                    testCase.FullyQualifiedName,
                    $" ({nextDistinctTestCaseCounterValue})");

                testCase.DisplayName = UniqueTestCaseSuffixRegex.Replace(
                    testCase.DisplayName,
                    $" ({nextDistinctTestCaseCounterValue})");
            }

            EnsureTestCaseNameIsUnique(testCase);
        }
    }
}
