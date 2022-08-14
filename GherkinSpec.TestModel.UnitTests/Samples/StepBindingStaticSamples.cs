namespace GherkinSpec.TestModel.UnitTests.Samples
{
    [Steps]
    public static class StepBindingStaticSamples
    {
        [Given("a plain (.+) in a non-static method")]
        public static void GivenANonStaticPlainTextMatch(string _)
        {
        }
    }
}