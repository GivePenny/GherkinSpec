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

namespace GherkinSpec.TestAdapter.Execution
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
                    .Concat(testData.Rule?.Background?.Steps ?? Enumerable.Empty<IStep>())
                    .Any();

                if (!hasAnySteps)
                {
                    MarkTestAsSkipped(testResult);
                }
                else
                {
                    using (var serviceScope = testRunContext.ServiceProvider.CreateScope())
                    {
                        // Before Scenario hooks should run here (see https://docs.cucumber.io/gherkin/reference/#background)
                        // > A Background is run before each scenario, but after any Before hooks. In your feature file, put the Background before the first Scenario.

                        IEnumerable<IStep> allScenarioSteps = testData.Feature.Background.Steps;

                        if (testData.Rule != null)
                        {
                            allScenarioSteps = allScenarioSteps.Concat(testData.Rule.Background.Steps);
                        }

                        allScenarioSteps = allScenarioSteps.Concat(testData.Scenario.Steps);

                        await ExecuteSteps(serviceScope.ServiceProvider, testResult, allScenarioSteps, testData, testRunContext)
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
            var scenarioExecutionStartTime = DateTime.UtcNow;
            var stepsIterator = new StepsIterator(steps);

            foreach (var step in stepsIterator.Iterate())
            {
                testResult.Messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{step.Title}{Environment.NewLine}"));

                var stepBinding = stepBinder.GetBindingFor(step, testData.Assembly);
                var executionStrategy = StepExecutionStrategyFactory.GetFor(stepBinding);

                try
                {
                    await executionStrategy
                        .Execute(stepBinding, serviceProvider, testResult.Messages, testRunContext)
                        .ConfigureAwait(false);

                    testResult.Messages.Add(
                        new TestResultMessage(
                            TestResultMessage.StandardOutCategory,
                            $"{StepLogIndent}Completed at {DateTime.UtcNow:o}{Environment.NewLine}{Environment.NewLine}"));
                }
                catch (Exception exception)
                {
                    if (ScenarioFailureIsTerminal(
                        testData.Scenario,
                        scenarioExecutionStartTime,
                        stepsIterator))
                    {
                        testResult.Messages.Add(
                            new TestResultMessage(
                                TestResultMessage.StandardOutCategory,
                                $"{StepLogIndent}Failed at {DateTime.UtcNow:o}{Environment.NewLine}{Environment.NewLine}"));

                        testResult.Messages.Add(
                            new TestResultMessage(
                                TestResultMessage.StandardErrorCategory,
                                $"{exception}{Environment.NewLine}"));

                        throw;
                    }

                    testResult.Messages.Add(
                        new TestResultMessage(
                                TestResultMessage.StandardOutCategory,
                                $"{StepLogIndent}Failed at {DateTime.UtcNow:o}, waiting and retrying scenario from last When step{Environment.NewLine}"));

                    await Task.Delay(testData.Scenario.EventuallyConsistentConfiguration.RetryInterval).ConfigureAwait(false);
                }
            }
        }

        private bool TryGoToMostRecentWhenStep(StepsIterator stepsIterator)
        {
            try
            {
                stepsIterator.GoToMostRecentWhenStep();
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool ScenarioFailureIsTerminal(Scenario scenario, DateTime scenarioExecutionStartTime, StepsIterator stepsIterator)
        {
            if (!scenario.IsEventuallyConsistent
                || scenario.EventuallyConsistentConfiguration == null
                || DateTime.UtcNow >= scenarioExecutionStartTime.Add(scenario.EventuallyConsistentConfiguration.Within)
                || !TryGoToMostRecentWhenStep(stepsIterator))
            {
                return true;
            }

            return false;
        }
    }
}
