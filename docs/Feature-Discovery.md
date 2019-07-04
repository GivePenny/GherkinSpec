# Feature Discovery

Feature files contain scenarios and Given, When, Then steps such as this example:

```gherkin
Feature: Addition

Scenario: Add two numbers together
    Given I have 5 apples
    When I add 6 more
    Then the result should be 11
```

Feature files must be compiled into the test project's output as an Embedded Resource or they will not be detected and will not appear in the Test Explorer tree of tests.  This is most easily achieved by adding this to the `csproj` file (this assumes that all your feature files are in a folder called Features):

```xml
  <ItemGroup>
    <None Remove="Features/**/*.feature" />
    <EmbeddedResource Include="Features/**/*.feature" />
  </ItemGroup>
```

Features can also be placed in deeper subfolders.  When scanning the project for features to build the Test Explorer tree in Visual Studio or Visual Studio Code, GherkinSpec doesn't show any common folder that ALL feature files are somewhere inside, even if they are in subfolders.  This is to avoid unneeded clicking around the Test Explorer tree.  It avoids showing a Features folder underneath every test project, for example.  If some features are outside of a certain folder, then that folder will be shown to distinguish between features outside it and features inside it.  The easiest way to understand this is to try it - create multiple feature files and move some into a subfolder.

## Special Tags

Features and scenarios can be tagged with metadata, for example:

```gherkin
Feature: Addition

@ignore, @category(Smoke Test)
Scenario: Add two numbers together
    Given I have 5 apples
    When I add 6 more
    Then the result should be 11
```

In the above example, the scenario is tagged @ignore, meaning that the test will be skipped during execution.  It is also categorised - it will show in Visual Studio's Test Explorer grouped by that category name if the Visual Studio option to "group by trait" is enabled.  Any other scenarios and features tagged with the same category will be grouped along with it.  This can be useful when selecting sets of tests to run from the `dotnet test` command line.  See the `dotnet --help` CLI documentation for information about test filtering.
