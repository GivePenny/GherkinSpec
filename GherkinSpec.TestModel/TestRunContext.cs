using System;

namespace GherkinSpec.TestModel
{
    public class TestRunContext
    {
        public TestRunContext(IServiceProvider defaultServiceProvider, ITestLogAccessor logger)
        {
            ServiceProvider = defaultServiceProvider;
            Logger = logger;
        }

        public IServiceProvider ServiceProvider { get; set; }

        public ITestLogAccessor Logger { get; }

        public EventualConfiguration EventualSuccess { get; } = new();

        public int MaximumSimultaneousTestCases { get; set; } = 20;
    }
}
