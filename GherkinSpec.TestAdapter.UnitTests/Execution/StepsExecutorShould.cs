using GherkinSpec.Model;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestAdapter.UnitTests.Samples;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.UnitTests.Execution
{
    [TestClass]
    public class StepsExecutorShould
    {
        private readonly Mock<IStepBinder> mockStepBinder = new Mock<IStepBinder>();
        private readonly Mock<IMessageLogger> mockLogger = new Mock<IMessageLogger>();
        private Assembly testAssembly;
        private TestCase testCase;
        private TestRunContext testRunContext;
        private StepsExecutor stepsExecutor;

        [TestInitialize]
        public void Setup()
        {
            testAssembly = Assembly.GetExecutingAssembly();
            testCase = new TestCase("Feature.Scenario", TestExecutor.ExecutorUriStronglyTyped, "SourceAssembly");
            stepsExecutor = new StepsExecutor(mockStepBinder.Object);

            testRunContext = new TestRunContext(
                new DefaultServiceProvider(),
                new Mock<ITestLogAccessor>().Object);

            testRunContext.EventualSuccess.DelayBetweenAttempts = TimeSpan.FromMilliseconds(250);
        }

        [TestMethod]
        public async Task ExecuteFeatureBackgroundStepsThenScenarioSteps()
        {
            var backgroundStep = new GivenStep("Feature background step", DataTable.Empty, null);
            var scenarioStep = new GivenStep("Scenario step", DataTable.Empty, null);

            var testFeature = new Feature("Feature", null,
                new Background(
                    new[] { backgroundStep },
                    0),
                new[]
                {
                    new Scenario(
                        "Scenario",
                        new[] { scenarioStep },
                        1,
                        Enumerable.Empty<Tag>())
                },
                Enumerable.Empty<ScenarioOutline>(),
                Enumerable.Empty<Rule>(),
                Enumerable.Empty<Tag>());

            var testData = new DiscoveredTestData(testAssembly, testFeature, null, testFeature.Scenarios.First());

            var invocationOrder = 0;
            var mockBackgroundStepMapping = new Mock<IStepBinding>();
            mockStepBinder
                .Setup(m => m.GetBindingFor(backgroundStep, testAssembly))
                .Returns(mockBackgroundStepMapping.Object)
                .Callback(() => Assert.AreEqual(0, invocationOrder++));

            var mockScenarioStepMapping = new Mock<IStepBinding>();
            mockStepBinder
                .Setup(m => m.GetBindingFor(scenarioStep, testAssembly))
                .Returns(mockScenarioStepMapping.Object)
                .Callback(() => Assert.AreEqual(1, invocationOrder++));

            var testResult = await stepsExecutor.Execute(testCase, testData, testRunContext, mockLogger.Object);
            
            mockBackgroundStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
            mockScenarioStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
        }

        [TestMethod]
        public async Task ExecuteFeatureBackgroundStepsThenRuleBackgroundStepsThenRuleScenarioSteps()
        {
            var featureBackgroundStep = new GivenStep("Feature background step", DataTable.Empty, null);
            var ruleBackgroundStep = new GivenStep("Rule background step", DataTable.Empty, null);
            var ruleScenarioStep = new GivenStep("Scenario step", DataTable.Empty, null);

            var testFeature = new Feature("Feature", null,
                new Background(
                    new[] { featureBackgroundStep },
                    0),
                Enumerable.Empty<Scenario>(),
                Enumerable.Empty<ScenarioOutline>(),
                new[]
                {
                    new Rule(
                        "Rule",
                        new Background(
                            new[] { ruleBackgroundStep },
                            0),
                        new[]
                        {
                            new Scenario(
                                "Scenario",
                                new[] { ruleScenarioStep },
                                1,
                                Enumerable.Empty<Tag>())
                        },
                        Enumerable.Empty<ScenarioOutline>(),
                        Enumerable.Empty<Tag>())
                },
                Enumerable.Empty<Tag>());

            var testData = new DiscoveredTestData(testAssembly, testFeature, testFeature.Rules.First(), testFeature.Rules.First().Scenarios.First());

            var invocationOrder = 0;
            var mockFeatureBackgroundStepMapping = new Mock<IStepBinding>();
            mockStepBinder
                .Setup(m => m.GetBindingFor(featureBackgroundStep, testAssembly))
                .Returns(mockFeatureBackgroundStepMapping.Object)
                .Callback(() => Assert.AreEqual(0, invocationOrder++));

            var mockRuleBackgroundStepMapping = new Mock<IStepBinding>();
            mockStepBinder
                .Setup(m => m.GetBindingFor(ruleBackgroundStep, testAssembly))
                .Returns(mockRuleBackgroundStepMapping.Object)
                .Callback(() => Assert.AreEqual(1, invocationOrder++));

            var mockRuleScenarioStepMapping = new Mock<IStepBinding>();
            mockStepBinder
                .Setup(m => m.GetBindingFor(ruleScenarioStep, testAssembly))
                .Returns(mockRuleScenarioStepMapping.Object)
                .Callback(() => Assert.AreEqual(2, invocationOrder++));

            var testResult = await stepsExecutor.Execute(testCase, testData, testRunContext, mockLogger.Object);

            mockFeatureBackgroundStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
            mockRuleBackgroundStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
            mockRuleScenarioStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsEventuallySuccessfulUntilFailure()
        {
            var standardMessages = await PerformEventuallyConsistentStepsTest(
                typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("WhenAnExceptionIsThrownMarkedEventuallySuccessful"));

            Assert.AreEqual(4, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and trying again{Environment.NewLine}", standardMessages[1].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and trying again{Environment.NewLine}", standardMessages[2].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at {Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsEventuallySuccessfulUntilSuccess()
        {
            var standardMessages = await PerformEventuallyConsistentStepsTest(
                typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("SucceedsOnSecondCallMarkedEventuallySucceeds"));

            Assert.AreEqual(3, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and trying again{Environment.NewLine}", standardMessages[1].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[2].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsMustNotEventuallyFailUntilFailure()
        {
            var standardMessages = await PerformEventuallyConsistentStepsTest(
                typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("FailsOnSecondCallMarkedAsMustNotEventuallyFail"));

            Assert.AreEqual(3, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Passed at , waiting and checking again{Environment.NewLine}", standardMessages[1].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at {Environment.NewLine}{Environment.NewLine}", standardMessages[2].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsMustNotEventuallyFailUntilTimeLimit()
        {
            var standardMessages = await PerformEventuallyConsistentStepsTest(
                typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("AlwaysSucceedsMarkedMustNotEventuallyFail"));

            Assert.AreEqual(4, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Passed at , waiting and checking again{Environment.NewLine}", standardMessages[1].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Passed at , waiting and checking again{Environment.NewLine}", standardMessages[2].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
        }

        [TestMethod]
        public async Task RetryTheScenarioMarkedAsEventuallyConsistentFromTheWhenStep()
        {
            var standardMessages = await PerformEventuallyConsistentScenarioTest(
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("GivenSomeSetup"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("WhenSomeEventuallyConsistentServiceIsCalled"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("ThenThisEventuallySucceedsAtScenarioLevel"));

            Assert.AreEqual(14, standardMessages.Length);
            Assert.AreEqual($"Given{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[2].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[4].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[5].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[6].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[7].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[8].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[9].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[10].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[11].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[12].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[13].Text);
        }

        [TestMethod]
        public async Task RetryTheScenarioMarkedAsEventuallyConsistentFromTheWhenStepUntilWithinHasPassed()
        {
            var standardMessages = await PerformEventuallyConsistentScenarioTest(
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("GivenSomeSetup"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("WhenThisServiceDoesNotBecomeConsistentInTime"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("ThenThisEventuallySucceedsAtScenarioLevel"));

            Assert.AreEqual(26, standardMessages.Length);
            Assert.AreEqual($"Given{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[2].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[4].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[5].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[6].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[7].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[8].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[9].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[10].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[11].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[12].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[13].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[14].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[15].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[16].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[17].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[18].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[19].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[20].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at , waiting and retrying scenario from last When step{Environment.NewLine}", standardMessages[21].Text);
            Assert.AreEqual($"When{Environment.NewLine}", standardMessages[22].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Completed at {Environment.NewLine}{Environment.NewLine}", standardMessages[23].Text);
            Assert.AreEqual($"Then{Environment.NewLine}", standardMessages[24].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at {Environment.NewLine}{Environment.NewLine}", standardMessages[25].Text);
        }

        [TestMethod]
        public async Task NotRetryTheScenarioMarkedAsEventuallyConsistentWhenFailureOccursBeforeAWhenStep()
        {
            var standardMessages = await PerformEventuallyConsistentScenarioTest(
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("GivenThisFails"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("WhenThisServiceDoesNotBecomeConsistentInTime"),
                typeof(StepBindingEventuallyConsistentScenarioSamples).GetMethod("ThenThisEventuallySucceedsAtScenarioLevel"));

            Assert.AreEqual(2, standardMessages.Length);
            Assert.AreEqual($"Given{Environment.NewLine}", standardMessages[0].Text);
            Assert.That.MessagesAreEqualIgnoringTimestamp($"  Failed at {Environment.NewLine}{Environment.NewLine}", standardMessages[1].Text);
        }

        private async Task<TestResultMessage[]> PerformEventuallyConsistentStepsTest(MethodInfo method)
        {
            var scenarioStep = new WhenStep("When an exception is thrown", DataTable.Empty, null);
            var testFeature = new Feature(
                "Feature",
                null,
                Background.Empty,
                new[]
                {
                    new Scenario(
                        "Scenario",
                        new[] { scenarioStep },
                        1,
                        Enumerable.Empty<Tag>())
                },
                Enumerable.Empty<ScenarioOutline>(),
                Enumerable.Empty<Rule>(),
                Enumerable.Empty<Tag>());

            var testData = new DiscoveredTestData(testAssembly, testFeature, null, testFeature.Scenarios.First());
            var binding = new StepBinding(scenarioStep, method, Array.Empty<object>());

            mockStepBinder
                .Setup(m => m.GetBindingFor(scenarioStep, testAssembly))
                .Returns(binding);

            var testResult = await stepsExecutor.Execute(testCase, testData, testRunContext, mockLogger.Object);

            return testResult
                .Messages
                .Where(
                    message => message.Category == TestResultMessage.StandardOutCategory)
                .ToArray();
        }

        private async Task<TestResultMessage[]> PerformEventuallyConsistentScenarioTest(MethodInfo givenMethod, MethodInfo whenMethod, MethodInfo thenMethod)
        {
            var givenStep = new GivenStep("Given", DataTable.Empty, null);
            var whenStep = new WhenStep("When", DataTable.Empty, null);
            var thenStep = new ThenStep("Then", DataTable.Empty, null);
            var testFeature = new Feature(
                "Feature",
                null,
                Background.Empty,
                new[]
                {
                    new Scenario(
                        "Scenario",
                        new IStep[] { givenStep, whenStep, thenStep },
                        1,
                        new List<Tag> { new Tag("eventuallyConsistent(retryInterval=00:00:01;within=00:00:05)") })
                },
                Enumerable.Empty<ScenarioOutline>(),
                Enumerable.Empty<Rule>(),
                Enumerable.Empty<Tag>());

            var testData = new DiscoveredTestData(testAssembly, testFeature, null, testFeature.Scenarios.First());
            var givenStepBinding = new StepBinding(givenStep, givenMethod, Array.Empty<object>());
            var whenStepBinding = new StepBinding(whenStep, whenMethod, Array.Empty<object>());
            var thenStepBinding = new StepBinding(thenStep, thenMethod, Array.Empty<object>());

            mockStepBinder
                .Setup(m => m.GetBindingFor(givenStep, testAssembly))
                .Returns(givenStepBinding);

            mockStepBinder
                .Setup(m => m.GetBindingFor(whenStep, testAssembly))
                .Returns(whenStepBinding);

            mockStepBinder
                .Setup(m => m.GetBindingFor(thenStep, testAssembly))
                .Returns(thenStepBinding);

            var testResult = await stepsExecutor.Execute(testCase, testData, testRunContext, mockLogger.Object);

            return testResult
                .Messages
                .Where(
                    message => message.Category == TestResultMessage.StandardOutCategory)
                .ToArray();
        }
    }
}
