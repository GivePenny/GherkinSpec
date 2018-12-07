# Hooks

Hooks are methods that are automatically executed at various points of the test run.  Currently the following hooks are supported.

Any class containing hook methods must be decorated with the `[Steps]` attribute before the methods will be detected and executed.

## BeforeRun

This hook identifies a method that runs once before the whole test run.

The method must be `public static` and must take exactly one argument of type `TestRunContext`.  It can have a `void` return type, or `async Task`.

Typically this method is used to set up Dependency Injection and to load configuration.

For a more complete example, see the [feature-rich example code](https://github.com/GivePenny/GherkinSpec.ComplexExample)

Example:

```csharp
[Steps]
public class Hooks
{
    [BeforeRun]
    public static void Setup(TestRunContext context)
    {
        var services = new ServiceCollection();
        context.ServiceProvider = services
            .AddSingleton<MyThingForWholeTestRun>()
            .AddScoped<MyContextDataForSingleTestCase>()
            .AddTransient<CreatedFreshForEachStep>()
            .BuildServiceProvider();
    }
}
```

## AfterRun

This hook identifies a method that runs once immediately after the whole test run has finished.

Like the BeforeRun methods, AfterRun methods must be `public static` and must take exactly one argument of type `TestRunContext`.  It can have a `void` return type, or `async Task`.

Typically this method is used to dispose of any resources created.

Example:

```csharp
[Steps]
public class Hooks
{
    [AfterRun]
    public static void Teardown(TestRunContext context)
    {
        var typedProvider = (ServiceProvider)testRunContext.ServiceProvider;

        typedProvider.Dispose();
    }
}
```
