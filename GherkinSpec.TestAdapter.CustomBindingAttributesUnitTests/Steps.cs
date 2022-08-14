using GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests.CustomAttributes;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests;

[CustomSteps]
public class Steps
{
    [CustomGiven("something given")]
    public void GivenSomething()
    {
    }
    
    [CustomWhen("something when")]
    public void WhenSomething()
    {
    }
    
    [CustomThen("something then")]
    public void ThenSomething()
    {
    }
}