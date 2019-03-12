# Eventually Consistent Services

A message-based architecture (where messages are pushed onto queues and pulled off one at a time to be processed) is difficult to service-test in a clear fashion because steps often need to wait an indeterminable amount of time for the messages to be processed before results can be verified.  This is true of any asynchronous process being tested, and so any eventually-consistent service.

GherkinSpec includes basic support for modern message-based microservices in order to keep the "retry" complexity out of the test code.

Eventual consistency is supported at two levels, step and scenario.

The intended usage of step level eventual consistency is as follows:

* A service consumes an event from another service, performs some processing, then publishes a different event.
* A test exists for this behaviour: "... When event X is received, Then event Y is published."
* Step level eventual consistency may be applied on the Then step in this test to cause the Then step to be retried until the event has been published.

The intended usage of scenario level eventual consistency is as follows:

* A service accepts data via HTTP PUT. The service enqueues a command to process that data to immediately return to the caller.  The service then serves the processed data via HTTP GET.
* A test exists for this behaviour: "Given I have submitted data, When I retrieve that data, Then the data matches my expectation."
* Scenario level eventual consistency may be applied to this test scenario to cause the When and Then steps to be retried until the data has been processed and is being served via HTTP GET.

## Eventually consistent steps

To configure any of the attributes below, specifically the delay between attempts and the total number of retries, set properties of the 'EventualSuccess' property on the 'TestRunContext' in a `[BeforeRun]` hook.  For an example of accessing the test run context, see the [Hooks](Hooks.md) documentation page.

### [EventuallySucceeds] attribute

Any step definition can be decorated with an `[EventuallySucceeds]` attribute.  This causes the step to be automatically retried if an exception is thrown.

#### Motivation

The idea is that a service can be queried for the expected results and if the success criteria are not yet met then the query can be performed again automatically after a delay.  This repeats until the service's state is consistent with the expected outcome.  The number of retries and the delay between each attempt can be configured in the `TestRunContext`.

Multiple attempts are logged in the test output along with the final exception if all attempts fail.  See the "Viewing the log messages" section of the [logging documentation](Logging.md) for help finding the test output.

#### Example

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

### [MustNotEventuallyFail] attribute

Any step definition can be decorated with an `[MustNotEventuallyFail]` attribute.  This causes the step to be automatically retried continuously up until a configured time limit is reached.  The step must pass successfully every time it is executed.

#### Motivation

This allows testing of eventually-consistent services that must never perform a specific action.  For example, a service test may verify the guarantee that a service would never publish a particular event in certain circumstances.  There is an assumption that if a service has not performed an action within the maximum amount of time that it may otherwise take to perform it, then it will never do so.  For example: if an `[EventuallySucceeds]` step may take up to 30 seconds to pass, then an `[MustNotEventuallyFail]` step would need to pass for 30 seconds before the test can assume that the service will never perform that action.

Typically test and subject design should seek to avoid using this attribute as it will slow down test runs (although GherkinSpec tests using async/await are highly parallelised so the impact may not be as high as is first thought).  As an example of how to avoid using this attribute: perhaps the test subject can perform a different positive action (e.g. as well as *not* publishing some form of ProcessingCompleted event, it could also publish a ProcessingSkipped event that the test can check for instead).  This is a lengthy design discussion involving questions such as "is it safe" (e.g. might the subject accidentally publish both events?) and "should the subject contain code purely there to support testing"?

Multiple attempts are logged in the test output.  As previously mentioned, see the "Viewing the log messages" section of the [logging documentation](Logging.md) for help finding the test output.

#### Example

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

#### Example avoiding use of this attribute

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

## Eventually consistent scenarios

### @eventuallyConsistent tag

Any scenario or scenario outline may be decorated with the `@eventuallyConsistent` tag.  This causes the scenario to be automatically retried from the first When step preceding the failing step until a configured amount of time has passed.

#### Motivation

The idea is that a service can be queried in a When step, and then assertions made in following Then steps. If the success criteria are not yet met then the query can be performed again automatically after a delay.  This repeats until the service's state is consistent with the expected outcome.  The delay between each attempt and the amount of time in which the service is expected to become consistent can be configured in the tag.

Multiple attempts are logged in the test output along with the final exception if all attempts fail.  See the "Viewing the log messages" section of the [logging documentation](Logging.md) for help finding the test output.

#### Configuration

The `@eventuallyConsistent` tag supports configuration for either the amount of time in which the service is expected to become consistent, or the delay between attempts, or both.

To configure the amount of time in which the service is expected to become consistent, use the `within=<TimeSpan string>` parameter, for example: `@eventuallyConsistent(within=00:00:20)`.

To configure the delay between attempts, use the `retryInterval=<TimeSpan string>` parameter, for example: `@eventuallyConsistent(retryInterval=00:00:05)`.

Both configuration parameters are supported, separated by the `;` character, for example: `@eventuallyConsistent(within=00:00:20;retryInterval=00:00:05)`.

#### Gotchas

* Depending on your `@eventuallyConsistent` tag configuration, it may be possible for a scenario to pass after the specified `within` time.  For example, if `within` is configured to 30 seconds, and `retryInterval` is configured at 29 seconds, then if the scenario fails on the first pass but succeeds on the second, then it is likely that that success will be just over 30 seconds.
* Consider the case in which a step is decorated with `[EventuallySucceeds]` or `[MustNotEventuallyFail]` and the scenario is decorated with the `@eventuallyConsistent` tag.  If the step fails for all of the configured `EventualSuccess` retries, then the scenario will be retried from the previous When step.  In this case, if the step passes on a following scenario attempt, the test scenario will pass with the step having surpassed its `EventualSuccess` configuration (at least once).