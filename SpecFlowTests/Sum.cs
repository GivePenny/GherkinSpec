namespace SpecFlowTests;

public class Sum
{
    public Sum(int firstNumber, int secondNumber)
    {
        Result = firstNumber + secondNumber;
    }

    public int Result { get; }
}