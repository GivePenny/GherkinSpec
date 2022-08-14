using System.Linq;
using GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;
using GherkinSpec.TestAdapter.UnitTests.ReferencedAssembly;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests;

[TestClass]
public class StepsClassesShould
{
    [TestMethod]
    public void ReturnTheExpectedClassesAndBindingTypeWhenAnAssemblyBindingTypesAttributeIsFound()
    {
        var stepsClassesAndBindingTypes = StepsClasses.FindInAssemblyAndReferencedAssemblies(GetType().Assembly).ToArray();

        var expectedStepsClassForThisAssembly = stepsClassesAndBindingTypes.SingleOrDefault(s => s.Types.Contains(typeof(Steps)));
        
        Assert.IsNotNull(expectedStepsClassForThisAssembly.Types);
        Assert.IsNotNull(expectedStepsClassForThisAssembly.BindingTypes);
        
        Assert.AreEqual(
            typeof(CustomStepsAttribute),
            expectedStepsClassForThisAssembly.BindingTypes.StepsClassAttributeType);
        
        Assert.AreEqual(
            typeof(CustomGivenAttribute),
            expectedStepsClassForThisAssembly.BindingTypes.GivenStepAttributeType);
        
        Assert.AreEqual(
            typeof(CustomWhenAttribute),
            expectedStepsClassForThisAssembly.BindingTypes.WhenStepAttributeType);
        
        Assert.AreEqual(
            typeof(CustomThenAttribute),
            expectedStepsClassForThisAssembly.BindingTypes.ThenStepAttributeType);
        
        var expectedStepsClassForReferencedAssembly = stepsClassesAndBindingTypes
            .SingleOrDefault(
                s => s.Types.Contains(typeof(ReferencedAssemblyStepBindingSamples)));

        Assert.IsNotNull(expectedStepsClassForReferencedAssembly);
        
        Assert.AreEqual(
            typeof(StepsAttribute),
            expectedStepsClassForReferencedAssembly.BindingTypes.StepsClassAttributeType);
        
        Assert.AreEqual(
            typeof(GivenAttribute),
            expectedStepsClassForReferencedAssembly.BindingTypes.GivenStepAttributeType);
        
        Assert.AreEqual(
            typeof(WhenAttribute),
            expectedStepsClassForReferencedAssembly.BindingTypes.WhenStepAttributeType);
        
        Assert.AreEqual(
            typeof(ThenAttribute),
            expectedStepsClassForReferencedAssembly.BindingTypes.ThenStepAttributeType);
    }
}