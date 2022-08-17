using GherkinSpec.TestModel;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    internal class NullTestLogAccessor : ITestLogAccessor
    {
        public bool IsInRunningTest => true;

        public void LogStepInformation(string message)
        {
        }
    }
}
