using GherkinSpec.TestModel;
using TechTalk.SpecFlow;

namespace SpecFlowTests;

public class SpecFlowStepAttributeMatchExpressionSelector : IStepAttributeMatchExpressionSelector
{
    public string SelectMatchExpression(object attribute)
    {
        return ((StepDefinitionBaseAttribute)attribute).Regex;
    }
}