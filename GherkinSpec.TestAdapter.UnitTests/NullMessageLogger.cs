using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace GherkinSpec.TestAdapter.UnitTests
{
    class NullMessageLogger : IMessageLogger
    {
        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }
    }
}
