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