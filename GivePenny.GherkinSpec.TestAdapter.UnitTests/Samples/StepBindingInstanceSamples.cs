using GivePenny.GherkinSpec.Model;
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

        [Given("a table")]
        public void GivenATable(DataTable table)
        {
        }
    }
}
