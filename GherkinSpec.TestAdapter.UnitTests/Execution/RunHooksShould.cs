using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestAdapter.Execution;
using GherkinSpec.TestAdapter.UnitTests.Samples;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.UnitTests.Execution
{
    [TestClass]
    public class RunHooksShould
    {
        private TestRunContext context;
        private RunHooks runHooks;

        [TestInitialize]
        public void Setup()
        {
            context = new TestRunContext(
                new DefaultServiceProvider(),
                new Mock<ITestLogAccessor>().Object);
            runHooks = new RunHooks(context, new[] { Assembly.GetExecutingAssembly() });
        }

        [TestMethod]
        public async Task ExecuteMethodsMarkedForBeforeRun()
        {
            var initialCount = HookSamples.BeforeRunCallCount;
            await runHooks.ExecuteBeforeRun();
            Assert.AreEqual(initialCount + 1, HookSamples.BeforeRunCallCount);
        }

        [TestMethod]
        public async Task ExecuteMethodsMarkedForAfterRun()
        {
            var initialCount = HookSamples.AfterRunCallCount;
            await runHooks.ExecuteAfterRun();
            Assert.AreEqual(initialCount + 1, HookSamples.AfterRunCallCount);
        }
    }
}
