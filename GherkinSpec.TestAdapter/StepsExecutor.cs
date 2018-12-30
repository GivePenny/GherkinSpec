using GherkinSpec.Model;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter
{
    internal class StepsExecutor
    {
        public const string StepLogIndent = "  ";

        private readonly IStepBinder stepBinder;

        public StepsExecutor(IStepBinder stepBinder)
        {
            this.stepBinder = stepBinder;
        }

        public async Task<TestResult> Execute(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, IMessageLogger logger)
        {
            const double SmallestTimeRecognisedByTestRunnerInSeconds = 0.0005;

            var testResult = new TestResult(testCase);
            var startTicks = Stopwatch.GetTimestamp();

            try
            {
                (testRunContext.Logger as TestLogAccessor)?.SetCurrentTestResult(testResult);

                if (testData == null)
                {
                    throw new ArgumentNullException(nameof(testData));
                }

                var hasAnySteps = testData.Feature.Background.Steps
                    .Concat(testData.Scenario.Steps)
                    .Any();

                if (!hasAnySteps)
                {
                    MarkTestAsSkipped(testResult);
                }
                else
                {
                    using (var serviceScope = testRunContext.ServiceProvider.CreateScope())
                    {
                        await ExecuteSteps(serviceScope.ServiceProvider, testResult, testData.Feature.Background.Steps, testData, testRunContext)
                            .ConfigureAwait(false);

                        // Cucumber docs say run BeforeHooks here, which is odd as before the Background steps is probably more useful.  If we ever add support for [BeforeScenario] then we should consider making it configurable whether that means before or after the Background steps.

                        await ExecuteSteps(serviceScope.ServiceProvider, testResult, testData.Scenario.Steps, testData, testRunContext)
                            .ConfigureAwait(false);
                    }

                    testResult.Outcome = TestOutcome.Passed;
                }

                testResult.Duration = TimeSpan.FromSeconds(
                    Math.Max(
                        SmallestTimeRecognisedByTestRunnerInSeconds,
                        (Stopwatch.GetTimestamp() - startTicks) / Stopwatch.Frequency));
            }
            catch (Exception exception)
            {
                MarkTestAsFailed(testCase, testResult, exception, logger);
            }

            return testResult;
        }

        private static void MarkTestAsFailed(TestCase testCase, TestResult testResult, Exception exception, IMessageLogger logger)
        {
            logger.SendMessage(
                TestMessageLevel.Error,
                $"Exception occurred during test \"{testCase.DisplayName}\": {exception}");

            testResult.Outcome = TestOutcome.Failed;
            testResult.ErrorMessage = exception.Message;
            testResult.ErrorStackTrace = exception.StackTrace;
        }

        private static void MarkTestAsSkipped(TestResult testResult)
        {
            testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"No steps found in scenario.{Environment.NewLine}"));

            testResult.Outcome = TestOutcome.Skipped;
        }

        private async Task ExecuteSteps(
            IServiceProvider serviceProvider,
            TestResult testResult,
            IEnumerable<IStep> steps,
            DiscoveredTestData testData,
            TestRunContext testRunContext)
        {
            foreach (var step in steps)
            {
                testResult.Messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{step.Title}{Environment.NewLine}"));

                IStepBinding stepBinding = null;
                var attempts = 0;
                do
                {
                    attempts++;

                    try
                    {
                        stepBinding = stepBinder.GetBindingFor(step, testData.Assembly);

                        await stepBinding
                            .Execute(serviceProvider, testResult.Messages)
                            .ConfigureAwait(false);

                        break;
                    }
                    catch (Exception exception)
                    {
                        if (IsTerminal(exception, stepBinding, attempts, testRunContext))
                        {
                            testResult.Messages.Add(
                                new TestResultMessage(
                                    TestResultMessage.StandardOutCategory,
                                    $"{StepLogIndent}Failed{Environment.NewLine}{Environment.NewLine}"));

                            testResult.Messages.Add(
                                    new TestResultMessage(
                                        TestResultMessage.StandardErrorCategory,
                                        $"{exception}{Environment.NewLine}"));

                            throw;
                        }
                    }

                    testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"{StepLogIndent}Failed, waiting and trying again{Environment.NewLine}"));

                    await Task
                        .Delay(testRunContext.EventualSuccess.DelayBetweenAttempts)
                        .ConfigureAwait(false);

                } while (true);

                testResult.Messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{StepLogIndent}Completed{Environment.NewLine}{Environment.NewLine}"));
            }
        }

        private bool IsTerminal(Exception exception, IStepBinding stepBinding, int attempts, TestRunContext testRunContext)
            => exception is StepBindingException
                || stepBinding == null
                || !stepBinding.IsSuccessEventual
                || attempts >= testRunContext.EventualSuccess.MaximumAttempts;
    }
}
