# Logging

## How to

The `dotnet test` CLI and the Visual Studio Test Explorer both capture logging output from each test that runs and stores it alongside that test's results.  This means that, when many tests are running in parallel, log messages from different tests don't interweave each other in a confusing fashion in the console.  Rather, each test's messages can be viewed clearly without messages from other tests that happened to be running at the same time getting in the way.  

To log messages out, ask for an `ITestLogAccessor` in the constructor of the relevant steps class, and then use it during a method.

```csharp
[Steps]
public class CalculatorSteps
{
    private readonly ITestLogAccessor logger;

    public CalculatorSteps(ITestLogAccessor logger)
    {
        this.logger = logger;
    }

    [Then(@"the result should be (\d+)")]
    public async Task ThenTheResultShouldBe(int expectedResult)
    {
        // ...
        logger.LogStepInformation("Example log message:" + expectedResult);
        // ...
    }
}
```

For a complete example, see the [feature-rich example](https://github.com/GivePenny/GherkinSpec.ComplexExample).

## Viewing the log messages

To view the messages that were logged for a test, view the output of the test.  To do this in Visual Studio, select your test in the Test Explorer after running it and then click the _Output_ link.  In the screenshot below, the output link is in the bottom left corner and the results on the right show the logged line `Example log message:3` under the relevant step.

![Screenshot showing Visual Studio Test Explorer and a test ouput](OutputLogMessage.png)

## Logging with custom dependency injection

The above example works with the default, built-in dependency injection of GherkinSpec.  If a more complex dependency-injection framework is used then the `ITestLogAccessor` instance available on the `Logger` property of the `TestRunContext` must be registered as a singleton.  Don't create a new instance of the `ITestLogAccessor` as it will not receive the context of the currently executing test so will fail to log.  The [feature-rich example](https://github.com/GivePenny/GherkinSpec.ComplexExample) does that.  An example using .NET Core's Dependency Injection extenion is:

```csharp
[Steps]
public static class Dependencies
{
    [BeforeRun]
    public static void Setup(TestRunContext testRunContext)
    {
        var services = new ServiceCollection();
        testRunContext.ServiceProvider = services
            // ... set up other things, including step definitions, here
            .AddSingleton(testRunContext.Logger)
            .BuildServiceProvider();
    }

    [AfterRun]
    public static void Teardown(TestRunContext testRunContext)
    {
        var typedProvider = (ServiceProvider)testRunContext.ServiceProvider;

        typedProvider.Dispose();
    }
}
```

## Why wasn't .NET Core's ILogger used?

We considered the complexity of writing a wrapper for `dotnet test`'s logger - a wrapper that would implement ILogger, along with the associated logger provider and glue to make them easily available.  We also considered the cost of coupling GherkinSpec to certain versions of `Microsoft.Extensions.Logging.Abstractions`.  The coupling and complexity did not seem worth the perceived benefits at the stage.  Please raise an issue if you have a concrete use case and this decision is causing a problem.
