using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter
{
    [ExtensionUri(ExecutorUri)]
    public class TestExecutor : ITestExecutor
    {
        public const string ExecutorUri = "executor://GherkinSpec";
        public static readonly Uri ExecutorUriStronglyTyped = new Uri(ExecutorUri);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public void Cancel()
            => cancellationTokenSource.Cancel();

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

                RunTestCases(mappedTests, frameworkHandle)
                    .Wait();
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

                RunTestCases(tests, frameworkHandle)
                    .Wait();
            }
            catch (Exception exception)
            {
                frameworkHandle.SendMessage(
                    TestMessageLevel.Error,
                    $"Skipping test run because of an early exception: {exception}");

                tests.TryMarkAsFailed(frameworkHandle, exception.Message, exception.StackTrace);
            }
        }

        private async Task RunTestCases(IEnumerable<TestCase> mappedTests, IFrameworkHandle frameworkHandle)
        {
            using var defaultServiceProvider = new DefaultServiceProvider();
            var testRunContext = defaultServiceProvider.GetService<TestRunContext>();

            var testCaseExecutor = new TestCaseExecutor(
                testRunContext,
                stepsBinder => new StepsExecutor(stepsBinder));

            await testCaseExecutor
                .RunTestCases(mappedTests, frameworkHandle, cancellationTokenSource.Token)
                .ConfigureAwait(false);
        }
    }
}
