using GivePenny.GherkinSpec.TestAdapter.DependencyInjection;
using GivePenny.GherkinSpec.TestAdapter.UnitTests.Samples;
using GivePenny.GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading.Tasks;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class RunHooksShould
    {
        private TestRunContext context;
        private RunHooks runHooks;

        [TestInitialize]
        public void Setup()
        {
            context = new TestRunContext(new DefaultServiceProvider());
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
