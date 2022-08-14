namespace GherkinSpec.TestModel;

public class StepAttributeMatchExpressionSelector : IStepAttributeMatchExpressionSelector
{
    public string SelectMatchExpression(object attribute)
    {
        return ((IStepAttribute)attribute).MatchExpression;
    }
}