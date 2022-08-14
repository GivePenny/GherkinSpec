namespace GherkinSpec.TestModel;

public interface IStepAttributeMatchExpressionSelector
{
    string SelectMatchExpression(object attribute);
}