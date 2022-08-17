using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace GherkinSpec.TestAdapter
{
    internal static class TestCasesExtensions
    {
        public static void TryMarkAsFailed(this IEnumerable<TestCase> testCases, IFrameworkHandle frameworkHandle, string errorMessage, string errorStackTrace)
        {
            try
            {
                foreach (var testCase in testCases)
                {
                    testCase.MarkAsFailed(frameworkHandle, errorMessage, errorStackTrace);
                }
            }
            catch
            {
            }
        }

        public static void MarkAsNotFound(this IEnumerable<TestCase> testCases, IFrameworkHandle frameworkHandle)
        {
            foreach (var testCase in testCases)
            {
                testCase.MarkAsNotFound(frameworkHandle);
            }
        }
    }
}
