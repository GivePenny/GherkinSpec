using GherkinSpec.TestModel;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    class NullTestLogAccessor : ITestLogAccessor
    {
        public bool IsInRunningTest => true;

        public void LogStepInformation(string message)
        {
        }
    }
}
