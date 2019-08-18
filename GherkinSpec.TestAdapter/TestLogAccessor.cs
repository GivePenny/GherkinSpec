using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Threading;

namespace GherkinSpec.TestAdapter
{
    class TestLogAccessor : ITestLogAccessor
    {
        private readonly AsyncLocal<TestResult> asyncLocalTestResult = new AsyncLocal<TestResult>();

        internal void SetCurrentTestResult(TestResult result)
        {
            asyncLocalTestResult.Value = result;
        }

        public bool IsInRunningTest => asyncLocalTestResult.Value != null;

        public void LogStepInformation(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var testResult = asyncLocalTestResult.Value;
            if (testResult == null)
            {
                throw new InvalidOperationException("The test log cannot be accessed outside the acope of a running test.");
            }

            testResult.Messages.Add(
                new TestResultMessage(
                    TestResultMessage.StandardOutCategory,
                    $"{StepsExecutor.StepLogIndent}{message}{Environment.NewLine}"));
        }
    }
}
