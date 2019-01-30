# Eventually Consistent Services

A message-based architecture (where messages are pushed onto queues and pulled off one at a time to be processed) is difficult to service-test in a clear fashion because steps often need to wait an indeterminable amount of time for the messages to be processed before results can be verified.  This is true of any asynchronous process being tested, and so any eventually-consistent service.

GherkinSpec includes basic support for modern message-based microservices in order to keep the "retry" complexity out of the test code.

Further features are planned (see the Issues list).

To configure any of the attributes below, specifically the delay between attempts and the total number of retries, set properties of the 'EventualSuccess' property on the 'TestRunContext' in a `[BeforeRun]` hook.  For an example of accessing the test run context, see the [Hooks](Hooks.md) documentation page.

## [EventuallySucceeds] attribute

Any step definition can be decorated with an `[EventuallySucceeds]` attribute.  This causes the step to be automatically retried if an exception is thrown.

### Motivation

The idea is that a service can be queried for the expected results and if the success criteria are not yet met then the query can be performed again automatically after a delay.  This repeats until the service's state is consistent with the expected outcome.  The number of retries and the delay between each attempt can be configured in the `TestRunContext`.

Multiple attempts are logged in the test output along with the final exception if all attempts fail.  See the "Viewing the log messages" section of the [logging documentation](Logging.md) for help finding the test output.

### Example

```csharp
[Steps]
public class EventSubscriptionStepDefinitions
{
  [Then("the order is accepted")]
  [EventuallySucceeds]
  public void ThenTheOrderIsAccepted
  {
    // ... code here that checks to see if a particular event was published, e.g. an OrderAccepted event.
  }
}
```

## [MustNotEventuallyFail] attribute

Any step definition can be decorated with an `[MustNotEventuallyFail]` attribute.  This causes the step to be automatically retried continuously up until a configured time limit is reached.  The step must pass successfully every time it is executed.

### Motivation

This allows testing of eventually-consistent services that must never perform a specific action.  For example, a service test may verify the guarantee that a service would never publish a particular event in certain circumstances.  There is an assumption that if a service has not performed an action within the maximum amount of time that it may otherwise take to perform it, then it will never do so.  For example: if an `[EventuallySucceeds]` step may take up to 30 seconds to pass, then an `[MustNotEventuallyFail]` step would need to pass for 30 seconds before the test can assume that the service will never perform that action.

Typically test and subject design should seek to avoid using this attribute as it will slow down test runs (although GherkinSpec tests using async/await are highly parallelised so the impact may not be as high as is first thought).  As an example of how to avoid using this attribute: perhaps the test subject can perform a different positive action (e.g. as well as *not* publishing some form of ProcessingCompleted event, it could also publish a ProcessingSkipped event that the test can check for instead).  This is a lengthy design discussion involving questions such as "is it safe" (e.g. might the subject accidentally publish both events?) and "should the subject contain code purely there to support testing"?

Multiple attempts are logged in the test output.  As previously mentioned, see the "Viewing the log messages" section of the [logging documentation](Logging.md) for help finding the test output.

### Example

```csharp
[Steps]
public class EventSubscriptionStepDefinitions
{
  [Then("the order is never accepted")]
  [MustNotEventuallyFail]
  public void ThenTheOrderIsNeverAccepted
  {
    // ... code here that asserts that a particular event has not yet been published, e.g. an OrderAccepted event.
  }
}
```

### Example avoiding use of this attribute

```csharp
[Steps]
public class EventSubscriptionStepDefinitions
{
  [Then("the order is rejected")]
  [EventuallySucceeds]
  public void ThenTheOrderIsRejected
  {
    // ... code here that checks to see if a particular event was published, e.g. an OrderRejected event.
	// Note that checking that an OrderRejected event was published may not be as safe as checking that an OrderAccepted event is never published.
	// That design decision is left open for discussion.  GherkinSpec supports both approaches.
  }
}
```