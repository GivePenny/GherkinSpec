using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace GherkinSpec.TestAdapter
{
    static class TestCaseExtensions
    {
        public static DiscoveredTestData DiscoveredData(this TestCase testCase)
            => (DiscoveredTestData)testCase.LocalExtensionData;

        public static void MarkAsFailed(this TestCase testCase, IFrameworkHandle frameworkHandle, string errorMessage, string errorStackTrace)
            => frameworkHandle.RecordResult(
                new TestResult(testCase)
                {
                    Outcome = TestOutcome.Failed,
                    ErrorMessage = errorMessage,
                    ErrorStackTrace = errorStackTrace
                });

        public static void MarkAsSkipped(this TestCase testCase, IFrameworkHandle frameworkHandle)
            => frameworkHandle.RecordResult(
                new TestResult(testCase)
                {
                    Outcome = TestOutcome.Skipped
                });

        public static void MarkAsNotFound(this TestCase testCase, IFrameworkHandle frameworkHandle)
            => frameworkHandle.RecordResult(
                new TestResult(testCase)
                {
                    Outcome = TestOutcome.NotFound
                });
    }
}
