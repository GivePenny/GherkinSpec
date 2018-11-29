using GivePenny.GherkinSpec.TestModel;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests.Samples
{
    [Steps]
    public class StepBindingInstanceSamples
    {
        [Given("a plain (.+) in a non-static method")]
        public void GivenANonStaticPlainTextMatch(string value)
        {
        }
    }
}
