# Dependency Injection

## Default basic DI

By default, GherkinSpec plugs in a basic Dependency Injection framework.  This currently will create an instance of any class that has a public constructor that takes no parameters.  The instance lifetime is scoped (see the Scoped section below for more information).

If you need to use a class that itself has dependencies or other constructor arguments, or if you would like to use a different lifetime for instances then you need to plug in an external DI framework.  The next section describes how to do that.

## Optional complete DI

We decided not to create our own abstraction to support different Dependency Injection frameworks in GherkinSpec.  We adopted .NET Core's standard `IServiceProvider` and `IServiceScope` from [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/).

This means any that dependency injection framework can be used, as long as it works with .NET Core apps and supports the `IServiceProvider` and `IServiceScope` abstractions.  The examples here use Microsoft's own implementation.

### Examples

Dependency injection is configured through a `BeforeRun` hook.  For a brief code snippet, see the [documentation page for Hooks](Hooks.md).  For a complete code sample, see the [GherkinSpec.ComplexExample](https://github.com/GivePenny/GherkinSpec.ComplexExample) repository.

### Instance lifetimes

#### Scoped

This is probably the most useful lifetime mode for steps classes and shared data.  A scoped instance is an instance of a class that is dedicated to a single test.

One test run containing several tests will create several instances of scoped classes, each one isolated and available only to one test.

#### Singleton

Singleton instances are dedicated to the entire test run.  One instance is shared between all tests.

#### Transient

Transient instances are created new every time they are requested.  A class marked with `[Steps]` that is registered with a transient lifetime will be instantiated every time a Given, When or Then defined in that class is encountered during a test.  Typically this is a little wasteful and the Scoped lifetime makes more sense.

Steps that manipulate a database and require a refresh context may be defined in a steps class with a transient lifetime, taking a database context also with a transient lifetime as a constructor-argument dependency.

### Gotchas

* See [Database contexts with Scoped steps classes](DependencyInjection-DatabaseContexts.md)
