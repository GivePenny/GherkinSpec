using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestAdapter.UnitTests.Mocks;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.UnitTests.Execution
{
    [TestClass]
    public class TestCaseExecutorShould
    {
        [TestMethod]
        public async Task LimitMaximumTestCasesRunInParallel()
        {
            var testCasesInProgress = 0;

            async Task<Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult> CreateTask(TestCase testCase)
            {
                Interlocked.Increment(ref testCasesInProgress);

                await Task.Yield();

                Interlocked.Decrement(ref testCasesInProgress);

                return new Microsoft.VisualStudio.TestPlatform.ObjectModel.TestResult(testCase);
            }

            var serviceProvider = new DefaultServiceProvider();
            var context = new TestRunContext(
                serviceProvider,
                new NullTestLogAccessor())
            {
                MaximumSimultaneousTestCases = 1
            };

            var mockStepsExecutor = new Mock<IStepsExecutor>();

            var testCaseExecutor = new TestCaseExecutor(context, binder =>
            {
                if(testCasesInProgress > 1)
                {
                    Assert.Fail();
                }

                return mockStepsExecutor.Object;
            });

            var testCase1 = new TestCase()
            {
                LocalExtensionData = new DiscoveredTestData(Assembly.GetExecutingAssembly(), new MockFeature(), null, new MockScenario())
            };

            mockStepsExecutor
                .Setup(m => m.Execute(testCase1, It.IsAny<DiscoveredTestData>(), context, It.IsAny<IMessageLogger>()))
                .Returns(CreateTask(testCase1));

            var testCase2 = new TestCase()
            {
                LocalExtensionData = new DiscoveredTestData(Assembly.GetExecutingAssembly(), new MockFeature(), null, new MockScenario())
            };

            mockStepsExecutor
                .Setup(m => m.Execute(testCase2, It.IsAny<DiscoveredTestData>(), context, It.IsAny<IMessageLogger>()))
                .Returns(CreateTask(testCase2));

            await testCaseExecutor
                .RunTestCases(
                    new[] { testCase1, testCase2 },
                    Mock.Of<IFrameworkHandle>(),
                    CancellationToken.None)
                .ConfigureAwait(false);
        }
    }
}
