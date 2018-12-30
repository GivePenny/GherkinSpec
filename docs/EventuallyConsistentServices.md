# Eventually Consistent Services

A message-based architecture (where messages are pushed onto queues and pulled off one at a time to be processed) is difficult to service-test in a clear fashion because steps often need to wait an indeterminable amount of time for the messages to be processed before results can be verified.  This is true of any asynchronous process being tested, and so any eventually-consistent service.

GherkinSpec includes basic support for modern message-based microservices in order to keep the "retry" complexity out of the test code.

Further features are planned (see the Issues list).

## [EventuallySucceeds] attribute

Any step definition can be decorated with an `[EventuallySucceeds]` attribute.  This causes the step to be automatically retried if an exception is thrown.  The idea is that a service can be queried for the expected results and if the success criteria are not yet met then the query can be performed again automatically after a delay.  This repeats until the service's state is consistent with the expected outcome.  The number of retries and the delay between each attempt can be configured in the `TestRunContext`.

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

To configure the delay and number of retries, set properties of the 'EventualSuccess' property on the 'TestRunContext' in a `[BeforeRun]` hook.  For an example of accessing the test run context, see the [Hooks](Hooks.md) documentation page.