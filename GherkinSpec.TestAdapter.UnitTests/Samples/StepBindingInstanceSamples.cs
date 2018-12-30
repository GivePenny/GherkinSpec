using GherkinSpec.Model;
using GherkinSpec.TestModel;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.Samples
{
    [Steps]
    public class StepBindingInstanceSamples
    {
        int calls = 0;

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

        [Then("this eventually succeeds")]
        [EventuallySucceeds]
        public void ThenThisEventuallySucceeds()
        {
        }

        [Then("this eventually succeeds")]
        [EventuallySucceeds]
        public void SucceedsOnSecondCall()
        {
            calls++;
            if (calls < 2)
            {
                throw new InvalidOperationException("Service data is not yet consistent");
            }
        }
    }
}
