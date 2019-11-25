﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace GherkinSpec.TestAdapter.UnitTests.Mocks
{
    class NullMessageLogger : IMessageLogger
    {
        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
        }
    }
}
