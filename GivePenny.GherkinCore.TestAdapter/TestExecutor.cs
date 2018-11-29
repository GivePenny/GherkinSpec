using GivePenny.GherkinCore.TestAdapter.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GivePenny.GherkinCore.TestAdapter
{
    [ExtensionUri(ExecutorUri)]
    public class TestExecutor : ITestExecutor
    {
        public const string ExecutorUri = "executor://GivePenny.GherkinCore";
        public static readonly Uri ExecutorUriStronglyTyped = new Uri(ExecutorUri);

        private bool isCancelling;

        public void Cancel()
            => isCancelling = true;

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running tests");

            var sources = tests
                .Select(test => test.Source)
                .Distinct();

            var testsMappedToScenarios = TestDiscoverer
                .DiscoverTests(sources, frameworkHandle)
                .ToArray();

            var unmappedTests = tests
                .Where(test => !testsMappedToScenarios.Any(mappedTest => mappedTest.Id == test.Id));

            foreach (var unmappedTest in unmappedTests)
            {
                RecordTestNotFound(unmappedTest, frameworkHandle);
            }

            var mappedTests = tests
                .Select(
                    test => testsMappedToScenarios
                        .FirstOrDefault(
                            mappedTest => mappedTest.Id == test.Id))
                .Where(test => test != null);

            RunMappedTests(mappedTests, runContext, frameworkHandle);
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            RunMappedTests(TestDiscoverer.DiscoverTests(sources, frameworkHandle), runContext, frameworkHandle);
        }

        private void RunMappedTests(IEnumerable<TestCase> mappedTests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, "Running tests");

            using (var serviceProvider = new DefaultServiceProvider())
            {
                var testRunContext = new TestRunContext(serviceProvider);
                var methodMapper = new MethodMapper();

                var tasks = new List<Task>();
                
                foreach (var testCase in mappedTests)
                {
                    if (isCancelling)
                    {
                        frameworkHandle.SendMessage(TestMessageLevel.Informational, "Test run cancelled");
                        break;
                    }

                    tasks.Add(
                        RunMappedTest(testCase, (DiscoveredTestData)testCase.LocalExtensionData, testRunContext, methodMapper, runContext, frameworkHandle));
                }

                Task.WhenAll(tasks).Wait();
            }
        }

        private async Task RunMappedTest(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, MethodMapper methodMapper, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.SendMessage(TestMessageLevel.Informational, $"Starting test \"{testCase.DisplayName}\"");

            frameworkHandle.RecordStart(testCase);

            var executor = new StepsExecutor(methodMapper);
            var testResult = await executor
                .Execute(testCase, testData, testRunContext, frameworkHandle)
                .ConfigureAwait(false);

            frameworkHandle.RecordResult(testResult);
        }

        private void RecordTestNotFound(TestCase testCase, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.RecordResult(
                new TestResult(testCase)
                {
                    Outcome = TestOutcome.NotFound
                });
        }
    }
}
