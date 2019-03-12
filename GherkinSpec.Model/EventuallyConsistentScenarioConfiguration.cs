using System;

namespace GherkinSpec.Model
{
    public class EventuallyConsistentScenarioConfiguration
    {
        public TimeSpan Within { get; internal set; } = TimeSpan.FromSeconds(20);
        public TimeSpan RetryInterval { get; internal set; } = TimeSpan.FromSeconds(5);
    }
}
