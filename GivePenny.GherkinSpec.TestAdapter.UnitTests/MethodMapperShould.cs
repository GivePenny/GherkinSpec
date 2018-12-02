using GivePenny.GherkinSpec.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests
{
    [TestClass]
    public class MethodMapperShould
    {
        private readonly Mock<IMessageLogger> mockLogger = new Mock<IMessageLogger>();

        // TODO CASE Does not match private methods?  Currently needs public/private class but public method - inconsistent?
        // TODO CASE "And a plain text match" rather than "Given a plain text match"
        // TODO CASE Review MethodMapper for other test cases needed after POC

        [TestMethod]
        public void FindMethodWithPlainTextGiven()
        {
            var mapper = new MethodMapper();
            var mapping = mapper.GetMappingFor(
                new GivenStep("Given a plain text match"),
                Assembly.GetExecutingAssembly());

            Assert.AreEqual("GivenAPlainTextMatch", mapping.Name);
        }

        [TestMethod]
        public void FindMethodWithSingleStringArgumentAndPreserveCase()
            => FindMethodWithSingleArgument("absolute Onions", "absolute Onions");

        [TestMethod]
        public void FindMethodWithSingleInt32Argument()
            => FindMethodWithSingleArgument("23", 23);

        [TestMethod]
        public void FindMethodWithSingleSingleArgument()
            => FindMethodWithSingleArgument("23.2", 23.2f);

        [TestMethod]
        public void FindMethodWithSingleBooleanArgumentOfTrueAndCaseInsensitive()
            => FindMethodWithSingleArgument("tRue", true);

        [TestMethod]
        public void FindMethodWithSingleBooleanArgumentOfFalseAndCaseInsensitive()
            => FindMethodWithSingleArgument("fAlse", false);

        private void FindMethodWithSingleArgument<TArgumentType>(string stepValue, TArgumentType expectedArgumentValue)
        {
            var mapper = new MethodMapper();
            var mapping = mapper.GetMappingFor(
                new GivenStep($"Given a single {typeof(TArgumentType).Name} match of {stepValue}"),
                Assembly.GetExecutingAssembly());

            Assert.AreEqual($"GivenASingle{typeof(TArgumentType).Name}Match", mapping.Name);
            Assert.AreEqual(1, mapping.Arguments.Length);
            Assert.AreEqual(expectedArgumentValue, mapping.Arguments[0]);
        }

        [TestMethod]
        public void FindMethodWithMultipleArguments()
        {
            var mapper = new MethodMapper();
            var mapping = mapper.GetMappingFor(
                new GivenStep("Given a single 'value-here' match and 'another here'"),
                Assembly.GetExecutingAssembly());

            Assert.AreEqual("GivenAMultipleStringMatch", mapping.Name);
            Assert.AreEqual(2, mapping.Arguments.Length);
            Assert.AreEqual("value-here", mapping.Arguments[0]);
            Assert.AreEqual("another here", mapping.Arguments[1]);
        }

        [TestMethod]
        public void FindNonStaticMethods()
        {
            var mapper = new MethodMapper();
            var mapping = mapper.GetMappingFor(
                new GivenStep("Given a plain value in a non-static method"),
                Assembly.GetExecutingAssembly());

            Assert.AreEqual("GivenANonStaticPlainTextMatch", mapping.Name);
            Assert.AreEqual(1, mapping.Arguments.Length);
        }

        [TestMethod]
        public void ThrowExceptionIfMethodRequiresMoreArgumentsThanTheStepAttributeProvides()
        {
            var mapper = new MethodMapper();

            var exception = Assert.ThrowsException<StepBindingException>(
                () =>
                    mapper.GetMappingFor(
                        new GivenStep("Given not enough captures to satisfy method arguments"),
                        Assembly.GetExecutingAssembly()));

            Assert.AreEqual(
                @"The step ""Given not enough captures to satisfy method arguments"" matches the method ""GivenNotEnoughCaptures"" on class ""GivePenny.GherkinSpec.TestAdapter.UnitTests.Samples.StepBindingStaticSamples"". That method expects 2 parameters but the step only provides 1.",
                exception.Message);
        }
    }
}
