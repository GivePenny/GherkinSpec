using GherkinSpec.TestModel;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.Samples
{
    [Steps]
    public class StepBindingEventuallyConsistentScenarioSamples
    {
        private int whenCalls;

        [Given("some setup")]
        public void GivenSomeSetup()
        {
        }

        [Given("this fails")]
        public void GivenThisFails()
        {
            throw new InvalidOperationException("Oops");
        }

        [When("some eventually consistent service is called")]
        public void WhenSomeEventuallyConsistentServiceIsCalled()
        {
            whenCalls++;
        }

        [When("this service does not become consistent in time")]
        public static void WhenThisServiceDoesNotBecomeConsistentInTime()
        {
        }

        [Then("this eventually succeeds at scenario level")]
        public void ThenThisEventuallySucceedsAtScenarioLevel()
        {
            if (whenCalls < 3)
            {
                throw new InvalidOperationException("Service data is not yet consistent");
            }
        }
    }
}
