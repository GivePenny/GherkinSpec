using System;

namespace GherkinSpec.TestModel
{
    public class EventualConfiguration
    {
        public int MaximumAttempts { get; set; } = 3;

        public TimeSpan DelayBetweenAttempts { get; set; } = TimeSpan.FromSeconds(7);
    }
}
