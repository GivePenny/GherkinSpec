namespace GherkinSpec.TestModel
{
    public interface ITestLogAccessor
    {
        void LogStepInformation(string message);

        bool IsInRunningTest { get; }
    }
}
