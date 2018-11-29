using GivePenny.GherkinSpec.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GivePenny.GherkinSpec.TestAdapter
{
    internal class StepsExecutor
    {
        private readonly MethodMapper methodMapper;

        public StepsExecutor(MethodMapper methodMapper)
        {
            this.methodMapper = methodMapper;
        }

        public async Task<TestResult> Execute(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, IMessageLogger logger)
        {
            var testResult = new TestResult(testCase);
            var startTicks = Stopwatch.GetTimestamp();

            try
            {
                if (testData == null)
                {
                    throw new ArgumentNullException(nameof(testData));
                }

                var steps = testData?.Scenario?.Steps;

                if (!steps.Any())
                {
                    MarkAsSkipped(testResult);
                }
                else
                {
                    await ExecuteSteps(testResult, steps, testData, testRunContext, logger);
                }
            }
            catch (Exception exception)
            {
                RecordFailedTest(testCase, testResult, exception, logger);
            }

            testResult.Duration = TimeSpan.FromSeconds(
                Math.Max(
                    0.001,
                    (Stopwatch.GetTimestamp() - startTicks) / Stopwatch.Frequency));

            return testResult;
        }

        private static void RecordFailedTest(TestCase testCase, TestResult testResult, Exception exception, IMessageLogger logger)
        {
            logger.SendMessage(
                TestMessageLevel.Error,
                $"Exception occurred during test \"{testCase.DisplayName}\": {exception}");

            testResult.Outcome = TestOutcome.Failed;
            testResult.ErrorMessage = exception.Message;
            testResult.ErrorStackTrace = exception.StackTrace;
        }

        private static void MarkAsSkipped(TestResult testResult)
        {
            testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"No steps found in scenario.{Environment.NewLine}"));

            testResult.Outcome = TestOutcome.Skipped;
        }

        private async Task ExecuteSteps(TestResult testResult, IEnumerable<IStep> steps, DiscoveredTestData testData, TestRunContext testRunContext, IMessageLogger logger)
        {
            using (var serviceScope = testRunContext.ServiceProvider.CreateScope())
            {
                foreach (var step in steps)
                {
                    testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"{step.Title}{Environment.NewLine}"));

                    try
                    {
                        var method = methodMapper.GetMappingFor(step, testData.Assembly, logger);
                        await method
                            .Execute(serviceScope, testResult.Messages)
                            .ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        testResult.Messages.Add(
                            new TestResultMessage(
                                TestResultMessage.StandardOutCategory,
                                $"Failed{Environment.NewLine}"));

                        testResult.Messages.Add(
                                new TestResultMessage(
                                    TestResultMessage.StandardErrorCategory,
                                    $"{exception}{Environment.NewLine}"));
                        throw;
                    }

                    testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"  Completed{Environment.NewLine}"));
                }
            }

            testResult.Outcome = TestOutcome.Passed;
        }
    }
}
