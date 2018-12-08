using GherkinSpec.Model;
using GherkinSpec.TestModel;

namespace GherkinSpec.TestAdapter.UnitTests.Samples
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

        [Given("a multi-line string")]
        public void GivenAMultiLineString(string multiLineString)
        {
        }
    }
}
