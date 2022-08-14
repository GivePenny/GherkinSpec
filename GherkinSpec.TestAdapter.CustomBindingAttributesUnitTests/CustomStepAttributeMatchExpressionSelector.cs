using GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;
using GherkinSpec.TestModel;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests;

public class CustomStepAttributeMatchExpressionSelector : IStepAttributeMatchExpressionSelector
{
    public string SelectMatchExpression(object attribute)
    {
        return ((ICustomStepAttribute)attribute).MatchRegex;
    }
}