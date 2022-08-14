using System.Linq;
using GherkinSpec.TestAdapter.UnitTests.ReferencedAssembly;
using GherkinSpec.TestModel.UnitTests.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void RegisterAllNonStaticStepsClassesInSpecifiedAssembly()
        {
            var services = new ServiceCollection();
            services.AddAllStepsClassesAsScoped(typeof(StaticStepsClass).Assembly);

            var expectedNonStaticStepsClasses = new[]
            {
                typeof(NonStaticStepsClass),
                typeof(ReferencedAssemblyHookSamples),
                typeof(ReferencedAssemblyStepBindingSamples)
            };
            
            CollectionAssert.AreEquivalent(
                expectedNonStaticStepsClasses,
                services.Select(s => s.ServiceType).ToList());
        }

        [TestMethod]
        public void DoesNotRegisterStaticStepsClassesInSpecifiedAssembly()
        {
            var services = new ServiceCollection();
            services.AddAllStepsClassesAsScoped(typeof(StaticStepsClass).Assembly);

            Assert.IsFalse(services.Any(descriptor => descriptor.ServiceType == typeof(StaticStepsClass)));
        }
    }
}