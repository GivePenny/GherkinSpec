using System;

namespace GherkinSpec.TestModel;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
public class BindingTypesAttribute : Attribute
{
    public Type StepsClassAttributeType { get; }
    public Type GivenStepAttributeType { get; }
    public Type WhenStepAttributeType { get; }
    public Type ThenStepAttributeType { get; }
    public Type StepAttributeMatchExpressionSelectorType { get; }

    public BindingTypesAttribute(
        Type stepsClassAttributeType = null,
        Type givenStepAttributeType = null,
        Type whenStepAttributeType = null,
        Type thenStepAttributeType = null,
        Type stepAttributeMatchExpressionSelectorType = null)
    {
        StepsClassAttributeType = stepsClassAttributeType ?? typeof(StepsAttribute);
        GivenStepAttributeType = givenStepAttributeType ?? typeof(GivenAttribute);
        WhenStepAttributeType = whenStepAttributeType ?? typeof(WhenAttribute);
        ThenStepAttributeType = thenStepAttributeType ?? typeof(ThenAttribute);
        
        StepAttributeMatchExpressionSelectorType = stepAttributeMatchExpressionSelectorType 
            ?? typeof(StepAttributeMatchExpressionSelector);
    }
}