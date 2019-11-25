using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Execution
{
    class TestCaseExecutor
    {
        private readonly TestRunContext testRunContext;
        private readonly Func<StepBinder, IStepsExecutor> stepsExecutorFactory;

        public TestCaseExecutor(TestRunContext testRunContext, Func<StepBinder, IStepsExecutor> stepsExecutorFactory)
        {
            this.testRunContext = testRunContext;
            this.stepsExecutorFactory = stepsExecutorFactory;
        }

        public async Task RunTestCases(IEnumerable<TestCase> testCases, IFrameworkHandle frameworkHandle, CancellationToken cancellationToken)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running tests");

            var runHooks = new RunHooks(
                testRunContext,
                testCases
                    .Select(
                        test => test
                            .DiscoveredData()
                            .Assembly)
                    .Distinct());

            await runHooks
                .ExecuteBeforeRun()
                .ConfigureAwait(false);

            using var simultaneousTestCasesSemaphore = new SemaphoreSlim(testRunContext.MaximumSimultaneousTestCases);
            var stepBinder = new StepBinder();

            var tasks = new List<Task>();

            foreach (var testCase in testCases)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test run cancelled");
                    break;
                }

                if (testCase.DiscoveredData().IsIgnored)
                {
                    testCase.MarkAsSkipped(frameworkHandle);
                    continue;
                }

                tasks.Add(
                    Run(testCase, testCase.DiscoveredData(), testRunContext, stepBinder, frameworkHandle, simultaneousTestCasesSemaphore));
            }

            await Task
                .WhenAll(tasks)
                .ConfigureAwait(false);

            await runHooks
                .ExecuteAfterRun()
                .ConfigureAwait(false);
        }

        private async Task Run(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, StepBinder stepBinder, IFrameworkHandle frameworkHandle, SemaphoreSlim simultaneousTestCasesSemaphore)
        {
            await simultaneousTestCasesSemaphore
                .WaitAsync()
                .ConfigureAwait(false);

            try
            {
                frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Starting test \"{testCase.DisplayName}\"");

                frameworkHandle.RecordStart(testCase);

                var executor = stepsExecutorFactory(stepBinder);

                var testResult = await executor
                    .Execute(testCase, testData, testRunContext, frameworkHandle)
                    .ConfigureAwait(false);

                // https://github.com/Microsoft/vstest/blob/master/src/Microsoft.TestPlatform.CrossPlatEngine/Adapter/TestExecutionRecorder.cs <- comments here seem to suggest that we need to call RecordEnd just before RecordResult  
                frameworkHandle.RecordEnd(testCase, testResult.Outcome);
                frameworkHandle.RecordResult(testResult);

                frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Finished test \"{testCase.DisplayName}\"");
            }
            finally
            {
                simultaneousTestCasesSemaphore.Release();
            }
        }
    }
}
