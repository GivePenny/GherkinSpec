using System;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;

[AttributeUsage(AttributeTargets.Method)]
public class CustomGivenAttribute : Attribute, ICustomStepAttribute
{
    public CustomGivenAttribute(
        string matchRegex)
    {
        MatchRegex = matchRegex;
    }

    public string MatchRegex { get; }
}