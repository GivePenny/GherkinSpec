# Configuration Reference

GherkinSpec is intended to be configuration-light.  This reference is provided for completeness and documents the configuration available on the `TestRunContext` instance that is passed to `BeforeRun` and `AfterRun` [hook methods](Hooks.md).

## ServiceProvider

Change the dependency-injection framework by setting this property to an instance of a class derived from `IServiceProvider`.  More on that is covered in the [Dependency Injection](DependencyInjection.md) article and the [Hooks article](Hooks.md).

## Logger

Provides access to the ILogger instance that the test framework will be writing messages to.  See the [Logging](Logging.md) article for more information.

## EventualSuccess

This has two child properties `MaximumAttempts` and `DelayBetweenAttempts`.  These are relevant to any steps that are marked as `EventuallySucceeds` or `MustNotEventuallyFail`.  The first property configures the maximum number of attempts before the test is assumed to have failed (unless it succeeds sooner).  The second property configures the time delay between attempts.

See [Eventually Consistent Services](EventuallyConsistentServices.md) for further reading.

## MaximumSimultaneousTestCases

GherkinSpec launches all test cases one at a time.  However, if any step returns a `Task` object then GherkinSpec will move on to launch the next test case whilst waiting for the former to complete.  This means that, although test cases are launched one at a time, many test cases may be "in progress" at the same time if they are waiting for something to complete (e.g. network calls, disk I/O or `Task.Delay()` calls).

For well-written test cases that are isolated from each other and have no side-effects that affect each other, this provides a tremendous amount of speed.  Typically this creates very fast suites of tests for (micro)services.

To avoid overloading the test subject (GherkinSpec is intended to be used for functional tests, not performance tests), GherkinSpec will by default only allow up to 20 test cases to be started and awaited in the above manner.  This is configurable through the `MaximumSimultaneousTestCases` property.

Note that if no steps return `Task` objects then test cases will not run in parallel - there is no opportunity for a "currently running test" to yield for another test to start.