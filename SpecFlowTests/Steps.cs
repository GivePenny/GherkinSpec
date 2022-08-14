using TechTalk.SpecFlow;

namespace SpecFlowTests;

[Binding]
public class Steps
{
    private int _firstNumber;
    private int _secondNumber;
    private Sum _sum;


    [Given("my first number is (\\d+)")]
    public void GivenMyFirstNumberIs(int firstNumber)
    {
        _firstNumber = firstNumber;
    }
    
    [Given("my second number is (\\d+)")]
    public void GivenMySecondNumberIs(int secondNumber)
    {
        _secondNumber = secondNumber;
    }

    [When("I sum my numbers")]
    public void WhenNumbersSummed()
    {
        _sum = new Sum(_firstNumber, _secondNumber);
    }

    [Then("the result is (\\d+)")]
    public void ThenTheResultIs(int expectedResult)
    {
        Assert.AreEqual(expectedResult, _sum.Result);
    }

    [Then("the result is not (\\d+)")]
    public void ThenTheResultIsNot(int expectedResult)
    {
        Assert.AreNotEqual(expectedResult, _sum.Result);
    }
}