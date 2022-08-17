using GherkinSpec.Model;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestAdapter.UnitTests.Samples;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.UnitTests.Binding
{
    [TestClass]
    public class StepBindingShould
    {
        private readonly Mock<IServiceProvider> mockServiceProvider = new();

        [TestMethod]
        public void IdentifyStepsMarkedAsEventuallySuccessfull()
        {
            var step = new ThenStep("this eventually succeeds", null, null);
            var method = typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("ThenThisEventuallySucceeds");
            var binding = new StepBinding(step, method, Array.Empty<object>());

            Assert.IsTrue(binding.IsSuccessEventual);
        }

        [TestMethod]
        public void IdentifyStepsNotMarkedAsEventuallySuccessfull()
        {
            var step = new GivenStep("a plain text match", null, null);
            var method = typeof(StepBindingStaticSamples).GetMethod("GivenAPlainTextMatch");
            var binding = new StepBinding(step, method, Array.Empty<object>());

            Assert.IsFalse(binding.IsSuccessEventual);
        }

        [TestMethod]
        public void IdentifyStepsMarkedAsMustNotEventuallyFail()
        {
            var step = new ThenStep("this eventually succeeds", null, null);
            var method = typeof(StepBindingEventuallyConsistentStepSamples).GetMethod("FailsOnSecondCallMarkedAsMustNotEventuallyFail");
            var binding = new StepBinding(step, method, Array.Empty<object>());

            Assert.IsTrue(binding.IsMarkedMustNotEventuallyFail);
        }

        [TestMethod]
        public void IdentifyStepsNotMarkedAsMustNotEventuallyFail()
        {
            var step = new GivenStep("a plain text match", null, null);
            var method = typeof(StepBindingStaticSamples).GetMethod("GivenAPlainTextMatch");
            var binding = new StepBinding(step, method, Array.Empty<object>());

            Assert.IsFalse(binding.IsMarkedMustNotEventuallyFail);
        }

        [TestMethod]
        public async Task NotWrapExceptionsInATargetInvocationExceptionWhenInvokedMethodThrowsAnException()
        {
            var step = new WhenStep("an exception is thrown", null, null);
            var method = typeof(StepBindingStaticSamples).GetMethod("WhenAnExceptionIsThrown");
            var binding = new StepBinding(step, method, Array.Empty<object>());

            var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => binding.Execute(
                    mockServiceProvider.Object,
                    new Collection<TestResultMessage>()));

            Assert.AreEqual("Hello", exception.Message);
        }
    }
}
