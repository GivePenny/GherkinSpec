using GherkinSpec.TestModel;
using SpecFlowTests;
using TechTalk.SpecFlow;
using GivenAttribute = TechTalk.SpecFlow.GivenAttribute;
using ThenAttribute = TechTalk.SpecFlow.ThenAttribute;
using WhenAttribute = TechTalk.SpecFlow.WhenAttribute;

[assembly: BindingTypes(
    typeof(BindingAttribute),
    typeof(GivenAttribute),
    typeof(WhenAttribute),
    typeof(ThenAttribute),
    typeof(SpecFlowStepAttributeMatchExpressionSelector))]