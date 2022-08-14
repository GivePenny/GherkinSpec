using GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests;
using GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;
using GherkinSpec.TestModel;

[assembly: BindingTypes(
    typeof(CustomStepsAttribute),
    typeof(CustomGivenAttribute),
    typeof(CustomWhenAttribute),
    typeof(CustomThenAttribute),
    typeof(CustomStepAttributeMatchExpressionSelector))]