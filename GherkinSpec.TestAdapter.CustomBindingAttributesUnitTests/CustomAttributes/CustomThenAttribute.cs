using System;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;

[AttributeUsage(AttributeTargets.Method)]
public class CustomThenAttribute : Attribute, ICustomStepAttribute
{
    public CustomThenAttribute(
        string matchRegex)
    {
        MatchRegex = matchRegex;
    }

    public string MatchRegex { get; }
}