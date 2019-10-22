using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter
{
    [ExtensionUri(ExecutorUri)]
    public class TestExecutor : ITestExecutor
    {
        public const string ExecutorUri = "executor://GherkinSpec";
        public static readonly Uri ExecutorUriStronglyTyped = new Uri(ExecutorUri);

        private bool isCancelling;

        public void Cancel()
            => isCancelling = true;

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            try
            {
                var sources = tests
                    .Select(test => test.Source)
                    .Distinct();

                var testsMappedToScenarios = new TestDiscoverer()
                    .DiscoverTests(sources, frameworkHandle)
                    .ToArray();

                var unmappedTests = tests
                    .Where(test => !testsMappedToScenarios.Any(mappedTest => mappedTest.Id == test.Id));

                unmappedTests.MarkAsNotFound(frameworkHandle);

                var mappedTests = tests
                    .Select(
                        test => testsMappedToScenarios
                            .FirstOrDefault(
                                mappedTest => mappedTest.Id == test.Id))
                    .Where(test => test != null);

                RunMappedTests(mappedTests, frameworkHandle).Wait();
            }
            catch (Exception exception)
            {
                frameworkHandle.SendMessage(
                    TestMessageLevel.Error,
                    $"Skipping test run because of an early exception: {exception}");

                tests.TryMarkAsFailed(frameworkHandle, exception.Message, exception.StackTrace);
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var tests = Enumerable.Empty<TestCase>();

            try
            {
                tests = new TestDiscoverer()
                    .DiscoverTests(sources, frameworkHandle)
                    .ToArray();

                RunMappedTests(tests, frameworkHandle).Wait();
            }
            catch (Exception exception)
            {
                frameworkHandle.SendMessage(
                    TestMessageLevel.Error,
                    $"Skipping test run because of an early exception: {exception}");

                tests.TryMarkAsFailed(frameworkHandle, exception.Message, exception.StackTrace);
            }
        }

        private async Task RunMappedTests(IEnumerable<TestCase> mappedTests, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running tests");

            using (var defaultServiceProvider = new DefaultServiceProvider())
            {
                var testRunContext = (TestRunContext)defaultServiceProvider.GetService(
                    typeof(TestRunContext));

                var runHooks = new RunHooks(
                    testRunContext,
                    mappedTests
                        .Select(
                            test => test
                                .DiscoveredData()
                                .Assembly)
                        .Distinct());

                await runHooks
                    .ExecuteBeforeRun()
                    .ConfigureAwait(false);

                var stepBinder = new StepBinder();

                var tasks = new List<Task>();
                
                foreach (var testCase in mappedTests)
                {
                    if (isCancelling)
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
                        RunMappedTest(testCase, testCase.DiscoveredData(), testRunContext, stepBinder, frameworkHandle));
                }

                await Task
                    .WhenAll(tasks)
                    .ConfigureAwait(false);

                await runHooks
                    .ExecuteAfterRun()
                    .ConfigureAwait(false);
            }
        }

        private async Task RunMappedTest(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, StepBinder stepBinder, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Starting test \"{testCase.DisplayName}\"");

            frameworkHandle.RecordStart(testCase);

            var executor = new StepsExecutor(stepBinder);

            var testResult = await executor
                .Execute(testCase, testData, testRunContext, frameworkHandle)
                .ConfigureAwait(false);

            // https://github.com/Microsoft/vstest/blob/master/src/Microsoft.TestPlatform.CrossPlatEngine/Adapter/TestExecutionRecorder.cs <- comments here seem to suggest that we need to call RecordEnd just before RecordResult  
            frameworkHandle.RecordEnd(testCase, testResult.Outcome);
            frameworkHandle.RecordResult(testResult);

            frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Finished test \"{testCase.DisplayName}\"");
        }
    }
}
