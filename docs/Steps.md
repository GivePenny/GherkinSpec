# Steps

Feature files contain scenarios and Given, When, Then steps such as this example:

```gherkin
Feature: Addition

Scenario: Add two numbers together
    Given I have 5 apples
    When I add 6 more
    Then the result should be 11
```

When the test is executed, GherkinSpec will look for public classes in your source code marked with the `[Steps]` attribute.  It will look through the public methods on those classes looking for any that are decorated with attributes such as `[Given(...)]`.

The classes and methods must be `public`.  They can be `static` but don't have to be.  Methods can be `void` or have a return type (the returned value is ignored).  Methods can optionally be `async Task`.

If a method is marked as `async Task`, another test may start whilst the current test is still running.  This is to make the most efficient use of the resources available.  Whilst one test is waiting for disk, database or network I/O, another test can use the processing time.  Tests should be written in a manner that means they don't interfere with each other, e.g. using uniquely generated data.

## Supported attributes

Attribute | Applied to | Purpose
--- | --- | ---
`[Steps]` | Class | Identifies a class that GherkinSpec should search
`[Given(...)]` | Method | Identifies a method that may match a Given step, including And and But steps that appear immediately following a Given.
`[When(...)]` | Method | Identifies a method that may match a When step, including And and But steps that appear immediately following a When.
`[Then(...)]` | Method | Identifies a method that may match a Then step, including And and But as per the last comment.

Other attributes may be added to methods in a `[Steps]` class, such as those that support [Hooks](Hooks.md).

## And and But steps

Sometimes the Gherkin feature file will contain And and But steps:

```gherkin
Feature: Addition

Scenario: Add two numbers together
    Given I have 5 apples
    And I have added 3 more
    When I add 6 more
    Then the result should be 14
```

When writing the C# step definitions for this test, there are no corresponding `[And]` and `[But]` attributes to decorate the methods with.  Instead, use the `[Given]`, `[When]` or `[Then]` attributes. The aim is to encourage each method, where possible, to be single-purpose and concentrate on either the test pre-requisites setup (Given, or "arrange"), the operation to test (When, or "act"), or the check and verification (Then, or "assert"). This helps step methods to be smaller and therefore reusable between scenarios more easily.  The purpose of the method is made more clear by the presence of attributes such as `[Given]` but would be confusing with an attribute such as `[And]`.  For a slightly deeper discussion on this point, see [issue #38](https://github.com/GivePenny/GherkinSpec/issues/38).

## Regular Expression matching

The argument to the Given, When and Then attributes is a .NET regular expression and can match parameters.  See the [simple example repository](https://github.com/GivePenny/GherkinSpec.SimpleExample) for an example showing steps that capture numeric arguments.  When capturing text (string) arguments, it may be a good idea to use delimiter characters to avoid matching more than is expected (see the example below).  Multi-argument steps can be a sign that a step is acting or asserting too much and therefore should be split into two smaller steps in order to improve re-usability.  The example below also demonstrates that.  In other cases, steps might legitimately require multiple arguments.

Example:
```gherkin
Feature: Error messages

Scenario: Make it break
	Given I have not entered a number
	When I press the add button
	Then the screen should show "Error" in the colour "red"
```

```csharp
[Then(@"the screen should show ""([^""]+)"" in the colour ""([^""]+)""")]
public void ThenTheScreenShouldShow(string text, string colourName)
{
	// TODO: Better code might split this into two separate steps, one to assert the text and one to assert the colour.
	// On the other hand, this step as it is demonstrates the need to wrap text parameters in delimiters and more strict regular expressions.
	// ...
}
```
