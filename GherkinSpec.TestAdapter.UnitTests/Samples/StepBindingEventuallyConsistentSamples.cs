using GherkinSpec.TestModel;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.Samples
{
    [Steps]
    public class StepBindingEventuallyConsistentSamples
    {
        int calls = 0;

        [Then("this eventually succeeds")]
        [EventuallySucceeds]
        public void ThenThisEventuallySucceeds()
        {
        }

        [Then("this eventually succeeds")]
        [EventuallySucceeds]
        public void SucceedsOnSecondCallMarkedEventuallySucceeds()
        {
            calls++;
            if (calls < 2)
            {
                throw new InvalidOperationException("Service data is not yet consistent");
            }
        }

        [When("an exception is thrown")]
        [EventuallySucceeds]
        public static void WhenAnExceptionIsThrownMarkedEventuallySuccessful()
        {
            throw new InvalidOperationException("Hello");
        }

        [Then("this eventually succeeds")]
        [MustNotEventuallyFail]
        public void AlwaysSucceedsMarkedMustNotEventuallyFail()
        {
        }

        [Then("this eventually fails")]
        [MustNotEventuallyFail]
        public void FailsOnSecondCallMarkedAsMustNotEventuallyFail()
        {
            calls++;
            if (calls > 1)
            {
                throw new InvalidOperationException("Service has suddenly decided to be jolly silly");
            }
        }
    }
}
