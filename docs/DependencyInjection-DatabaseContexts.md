# Database contexts with Scoped steps classes

Registering classes containing steps as Scoped means that only one instance of the class will be created per test.  All steps in one test will access the same object.  Usually this is useful, but occasionally you will want to access a transient-lifetime object from within that scoped-lifetime object.  One example of this is when accessing a database context or other data-access class based on the unit-of-work pattern (used typically to support transactions).  You may want to use one database context instance to set data up, and a new separate instance to query results at the end of the test.  This ensures that no cached data inside the setup instance is read by mistake when verifying the new state of the database.

The problem is not immediately obvious but is that the database context injected into the scoped steps instance would itself behave like a scoped instance, even if registered as transient.  This is because the scoped steps class is created once, so its constructor is executed once, so any dependencies injected into the constructor are only captured once as well.  This means that injecting a transient-lifetime object into a scoped-lifetime object usually means both are effectively scoped-lifetime.  In our example, this isn't desirable because now the same database context that sets up data at the start of the test is used to assert state at the end.  Caching and other effects may mean the results of the test cannot be trusted (e.g. false positive passes).

To solve this, you could use a factory method approach.  In the following example, the lifetime of the steps class is scoped, the lifetime of the factory method is therefore also scoped.  The method is called whenever a new database context is needed.  This allows the database context to have a transient lifetime, solving our problem.

This example uses Entity Framework.

```csharp
[Steps]
public static class Dependencies
{
    [BeforeRun]
    public static void Setup(TestRunContext testRunContext)
    {
        var services = new ServiceCollection();

        // ... omitted code to read configuration here ...

        testRunContext.ServiceProvider = services
            .AddScoped<DataSetupSteps>()
            .AddScoped<WebApiSteps>()
            .AddScoped<DataVerificationSteps>()
            .AddScoped<Func<DatabaseContext>>(
                sp =>
                    () => sp.GetRequiredService<DatabaseContext>())
            .AddDbContext<DatabaseContext>(
                options => options.UseSqlServer(
                    configuration["SqlDatabaseConnectionString"]))
            .BuildServiceProvider();
    }

    // ... omitted standard teardown/dispose code ...
}
```

```csharp
// First example showing semi-transient database context (one instance shared by all methods in the steps class, but other steps classes get their own new instance)
[Steps]
public class DataSetupSteps
{
    private readonly DatabaseContext _databaseContext;

    public DataSetupSteps(Func<DatabaseContext> databaseContextFactory)
    {
        // This creates a new instance of the database context for this class.  Other steps classes would have their own unique instance.
        _databaseContext = databaseContextFactory();
    }
}
```

```csharp
// Second example showing fully-transient database context (every method creates, uses and disposes its own database context instance)
// This is super-safe, but less performant.
[Steps]
public class DataSetupSteps
{
    private readonly Func<DatabaseContext> _databaseContextFactory;

    public DataSetupSteps(Func<DatabaseContext> databaseContextFactory)
    {
        _databaseContextFactory = databaseContextFactory;
    }

    [Given(@"data is set up")]
    public void GivenISetDataUp()
    {
        // This creates a new instance of the database context for this method.
        using(var databaseContext = _databaseContextFactory())
        {
            // Use the databaseContext to read/write data.
        }
    }
}
```