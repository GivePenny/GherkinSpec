using System;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;

[AttributeUsage(AttributeTargets.Method)]
public class CustomWhenAttribute : Attribute, ICustomStepAttribute
{
    public CustomWhenAttribute(
        string matchRegex)
    {
        MatchRegex = matchRegex;
    }

    public string MatchRegex { get; }
}