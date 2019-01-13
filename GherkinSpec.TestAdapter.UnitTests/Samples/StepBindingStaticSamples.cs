using GherkinSpec.TestModel;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.Samples
{
    [Steps]
    public static class StepBindingStaticSamples
    {
        [Given("a plain text match")]
        public static void GivenAPlainTextMatch()
        {
        }

        [Given("a single String match of (.*)")]
        public static void GivenASingleStringMatch(string text)
        {
        }

        [Given("a single '([^\']+)' match and '([^\']+)'")]
        public static void GivenAMultipleStringMatch(string text1, string text2)
        {
        }

        [Given("a single Int32 match of (.*)")]
        public static void GivenASingleInt32Match(int value)
        {
        }

        [Given("a single Single match of (.*)")]
        public static void GivenASingleSingleMatch(float value)
        {
        }

        [Given("a single Decimal match of (.*)")]
        public static void GivenASingleDecimalMatch(decimal value)
        {
        }

        [Given("a single Boolean match of (.*)")]
        public static void GivenASingleBooleanMatch(bool value)
        {
        }

        [Given("a single DateTime match of (.*)")]
        public static void GivenASingleDateTimeMatch(DateTime value)
        {
        }

        [Given(@"not enough (\w+) to satisfy method arguments")]
        public static void GivenNotEnoughCaptures(string argument1, string argument2)
        {
        }

        [When("an exception is thrown")]
        public static void WhenAnExceptionIsThrown()
        {
            throw new InvalidOperationException("Hello");
        }
    }
}
