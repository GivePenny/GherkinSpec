﻿using GivePenny.GherkinSpec.Model;
using GivePenny.GherkinSpec.TestAdapter.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class StepsExecutorShould
    {
        private readonly Mock<IMethodMapper> mockMethodMapper = new Mock<IMethodMapper>();
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
            testRunContext = new TestRunContext(new DefaultServiceProvider());
            stepsExecutor = new StepsExecutor(mockMethodMapper.Object);
        }

        [TestMethod]
        public async Task ExecuteFeatureBackgroundStepsThenScenarioSteps()
        {
            var backgroundStep = new GivenStep("Background step");
            var scenarioStep = new GivenStep("Scenario step");

            var testFeature = new Feature("Feature", null,
                new Background(
                    new[] { backgroundStep },
                    0),
                new[]
                {
                    new Scenario(
                        "Scenario",
                        new[] { scenarioStep },
                        1)
                });

            var testData = new DiscoveredTestData(testAssembly, testFeature, testFeature.Scenarios.First());

            var invocationOrder = 0;
            var mockBackgroundStepMapping = new Mock<IMethodMapping>();
            mockMethodMapper
                .Setup(m => m.GetMappingFor(backgroundStep, testAssembly))
                .Returns(mockBackgroundStepMapping.Object)
                .Callback(() => Assert.AreEqual(0, invocationOrder++));

            var mockScenarioStepMapping = new Mock<IMethodMapping>();
            mockMethodMapper
                .Setup(m => m.GetMappingFor(scenarioStep, testAssembly))
                .Returns(mockScenarioStepMapping.Object)
                .Callback(() => Assert.AreEqual(1, invocationOrder++));

            var testResult = await stepsExecutor.Execute(testCase, testData, testRunContext, mockLogger.Object);
            
            mockBackgroundStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
            mockScenarioStepMapping.Verify(m => m.Execute(It.IsAny<IServiceProvider>(), It.IsAny<Collection<TestResultMessage>>()), Times.Once);
        }
    }
}
