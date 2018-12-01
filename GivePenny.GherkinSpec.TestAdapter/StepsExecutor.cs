﻿using GivePenny.GherkinSpec.Model;
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

                var hasAnySteps = testData.Feature.Background.Steps
                    .Concat(testData.Scenario.Steps)
                    .Any();

                if (!hasAnySteps)
                {
                    MarkAsSkipped(testResult);
                }
                else
                {
                    using (var serviceScope = testRunContext.ServiceProvider.CreateScope())
                    {
                        await ExecuteSteps(serviceScope.ServiceProvider, testResult, testData.Feature.Background.Steps, testData, logger);

                        // TODO Cucumber docs say run BeforeHooks here.

                        await ExecuteSteps(serviceScope.ServiceProvider, testResult, testData.Scenario.Steps, testData, logger);
                    }

                    testResult.Outcome = TestOutcome.Passed;
                }
            }
            catch (Exception exception)
            {
                MarkAsFailed(testCase, testResult, exception, logger);
            }

            testResult.Duration = TimeSpan.FromSeconds(
                Math.Max(
                    0.001,
                    (Stopwatch.GetTimestamp() - startTicks) / Stopwatch.Frequency));

            return testResult;
        }

        private static void MarkAsFailed(TestCase testCase, TestResult testResult, Exception exception, IMessageLogger logger)
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

        private async Task ExecuteSteps(
            IServiceProvider serviceProvider,
            TestResult testResult,
            IEnumerable<IStep> steps,
            DiscoveredTestData testData,
            IMessageLogger logger)
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
                            .Execute(serviceProvider, testResult.Messages)
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
    }
}
