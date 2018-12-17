using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Threading;

namespace GherkinSpec.TestAdapter
{
    class TestLogAccessor : ITestLogAccessor
    {
        private AsyncLocal<TestResult> asyncLocalTestResult = new AsyncLocal<TestResult>();

        internal void SetCurrentTestResult(TestResult result)
        {
            asyncLocalTestResult.Value = result;
        }

        public void LogStepInformation(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            asyncLocalTestResult.Value.Messages.Add(
                new TestResultMessage(
                    TestResultMessage.StandardOutCategory,
                    $"{StepsExecutor.StepLogIndent}{message}{Environment.NewLine}"));
        }
    }
}
