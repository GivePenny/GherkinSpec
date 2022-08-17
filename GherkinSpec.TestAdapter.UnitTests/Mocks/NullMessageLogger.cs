using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    internal class NullMessageLogger : IMessageLogger
    {
        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }
    }
}
