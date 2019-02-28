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
            var standardMessages = await PerformEventuallyConsistentTest(
                typeof(StepBindingEventuallyConsistentSamples).GetMethod("WhenAnExceptionIsThrownMarkedEventuallySuccessful"));

            Assert.AreEqual(4, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.AreEqual($"  Failed, waiting and trying again{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"  Failed, waiting and trying again{Environment.NewLine}", standardMessages[2].Text);
            Assert.AreEqual($"  Failed{Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsEventuallySuccessfulUntilSuccess()
        {
            var standardMessages = await PerformEventuallyConsistentTest(
                typeof(StepBindingEventuallyConsistentSamples).GetMethod("SucceedsOnSecondCallMarkedEventuallySucceeds"));

            Assert.AreEqual(3, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.AreEqual($"  Failed, waiting and trying again{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"  Completed{Environment.NewLine}{Environment.NewLine}", standardMessages[2].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsMustNotEventuallyFailUntilFailure()
        {
            var standardMessages = await PerformEventuallyConsistentTest(
                typeof(StepBindingEventuallyConsistentSamples).GetMethod("FailsOnSecondCallMarkedAsMustNotEventuallyFail"));

            Assert.AreEqual(3, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.AreEqual($"  Passed, waiting and checking again{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"  Failed{Environment.NewLine}{Environment.NewLine}", standardMessages[2].Text);
        }

        [TestMethod]
        public async Task RetryStepsMarkedAsMustNotEventuallyFailUntilTimeLimit()
        {
            var standardMessages = await PerformEventuallyConsistentTest(
                typeof(StepBindingEventuallyConsistentSamples).GetMethod("AlwaysSucceedsMarkedMustNotEventuallyFail"));

            Assert.AreEqual(4, standardMessages.Length);
            Assert.AreEqual($"When an exception is thrown{Environment.NewLine}", standardMessages[0].Text);
            Assert.AreEqual($"  Passed, waiting and checking again{Environment.NewLine}", standardMessages[1].Text);
            Assert.AreEqual($"  Passed, waiting and checking again{Environment.NewLine}", standardMessages[2].Text);
            Assert.AreEqual($"  Completed{Environment.NewLine}{Environment.NewLine}", standardMessages[3].Text);
        }

        private async Task<TestResultMessage[]> PerformEventuallyConsistentTest(MethodInfo method)
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
    }
}
