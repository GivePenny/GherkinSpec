using GherkinSpec.Model;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Execution
{
    internal class ScenarioExecutor
    {
        private readonly IStepBinder stepBinder;

        public ScenarioExecutor(
            IStepBinder stepBinder)
        {
            this.stepBinder = stepBinder;
        }

        public async Task ExecuteSteps(IServiceProvider serviceProvider, TestResult testResult, IEnumerable<IStep> steps, DiscoveredTestData testData, TestRunContext testRunContext)
        {
            var startTime = DateTime.UtcNow;
            var stepsArray = steps as IStep[] ?? steps.ToArray();
            
            for (var stepIndex = 0; stepIndex < stepsArray.Length; stepIndex++)
            {
                var step = stepsArray.ElementAt(stepIndex);

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
                }
                catch (Exception exception)
                {
                    if (!testData.Scenario.IsEventuallyConsistent
                        || DateTime.UtcNow > startTime.Add(testData.Scenario.EventuallyConsistentConfiguration.Within))
                    {
                        testResult.Messages.Add(
                                    new TestResultMessage(
                                        TestResultMessage.StandardOutCategory,
                                        $"{StepsExecutor.StepLogIndent}Failed{Environment.NewLine}{Environment.NewLine}"));

                        testResult.Messages.Add(
                                new TestResultMessage(
                                    TestResultMessage.StandardErrorCategory,
                                    $"{exception}{Environment.NewLine}"));

                        throw;
                    }

                    var previousStepsInReverse = stepsArray.Take(stepIndex + 1).Reverse();
                    var lastWhenStep = previousStepsInReverse.First(s => s is WhenStep);
                    stepIndex = stepsArray.ToList().IndexOf(lastWhenStep);
                    await Task.Delay(testData.Scenario.EventuallyConsistentConfiguration.RetryInterval).ConfigureAwait(false);
                    continue;
                }

                testResult.Messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{StepsExecutor.StepLogIndent}Completed{Environment.NewLine}{Environment.NewLine}"));
            }
        }
    }
}
