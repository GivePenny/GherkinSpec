using System;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;

[AttributeUsage(AttributeTargets.Class)]
public class CustomStepsAttribute : Attribute
{
}