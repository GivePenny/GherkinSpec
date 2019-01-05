using GherkinSpec.TestModel.UnitTests.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GherkinSpec.TestModel.UnitTests
{
    [TestClass]
    public class ServiceCollectionExtensionsShould
    {
        [TestMethod]
        public void RegisterAllNonStaticStepsClassesInCallingAssembly()
        {
            var services = new ServiceCollection();
            services.AddAllStepsClassesAsScoped();

            Assert.AreEqual(1, services.Count(descriptor => descriptor.ServiceType == typeof(StepBindingInstanceSamples)));
        }

        [TestMethod]
        public void DoesNotRegisterStaticStepsClasses()
        {
            var services = new ServiceCollection();
            services.AddAllStepsClassesAsScoped();

            Assert.AreEqual(0, services.Count(descriptor => descriptor.ServiceType == typeof(StepBindingStaticSamples)));
        }
    }
}
