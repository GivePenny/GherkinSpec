# GherkinSpec

## Overview

A lightweight, cross-platform .NET Standard test adapter that discovers Gherkin tests from feature files and executes them.  Inspired by the giants of Cucumber, SpecFlow and BDDfy, filling a niche between them for microservice tests that are both lightweight (BDDfy) yet self-documenting in a natural language (SpecFlow/Cucumber).

Published package: https://www.nuget.org/packages/GivePenny.GherkinSpec.TestAdapter

![Screenshot showing Test Explorer, a Gherkin Feature file and C# steps](docs/Preview.png)

## Key features

* Fully cross-platform (.NET Standard)
* No generated code files (e.g. *.Designer.cs)
* Supports dependency injection

## Getting started

See [the simple example repository](https://github.com/GivePenny/GherkinSpec.SimpleExample), download the code, try it out.

1. Create a new .NET Core project for your tests.
2. Add Nuget package references to [GivePenny.GherkinSpec.TestAdapter](https://www.nuget.org/packages/GivePenny.GherkinSpec.TestAdapter) and [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk).
3. Add a plain-text file (ending in .feature) to your project, mark it as an Embedded Resource (either in the csproj file, or in Visual Studio's Properties pane)
4. Write your C# steps, tagging the class with a `[Steps]` attribute and your methods with `[Given]` or `[When]` or `[Then]`
5. Add a reference to a unit test framework of your choice if you would like to perform Assertions, for example [MSTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework)

Steps 1, 2 and 5 can be speeded up by creating your test project using this template in Visual Studio (you will still need to add a reference to GivePenny.GherkinSpec.TestAdapter).

![Screenshot showing a new .NET Core MS Test project](docs/MSTestProject.png)

## Full feature list

* Open source
* .NET CLI compatible (`dotnet test`)
* Visual Studio / Visual Studio Code compatible
* Azure DevOps compatible (test results reported)
* Supports `async`/`await`able steps
* Efficiently runs tests in parallel (if they contain `async` steps)

## Gotchas

* When creating a new .feature file, make sure that it is added as an Embedded Resource (see the [csproj of the simple example](https://github.com/GivePenny/GherkinSpec.SimpleExample/blob/master/GivePenny.GherkinSpec.SimpleExample.Tests/GivePenny.GherkinSpec.SimpleExample.Tests.csproj) - in Visual Studio this can also be set in the Properties pane when the file is selected)
* If you keep getting .designer.cs files appear, uninstall the SpecFlow Extension for Visual Studio or name your files .gherkin instead of .feature.
* Make sure you add the Test SDK package to your test project, again see the csproj for the simple example (`<PackageReference Include="Microsoft.NET.Test.Sdk" Version="15.9.0" />`).

## Roadmap

The project's next steps are (in no particular order):

* Further documentation (including contribution guidelines)
* Support for the full Gherkin syntax (currently only Feature, Scenario, Given, When and Then are supported)
* Full example showing dependency injection, configuration, logging and async/await.

## References and useful links

* [Full Gherkin syntax](https://docs.cucumber.io/gherkin/reference/)
* For VSCode Gherkin syntax highlighting try [other community extensions, such as this one](https://marketplace.visualstudio.com/items?itemName=stevejpurves.cucumber).