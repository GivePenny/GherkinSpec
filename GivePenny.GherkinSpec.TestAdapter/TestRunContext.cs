using System;

namespace GivePenny.GherkinSpec.TestAdapter
{
    public class TestRunContext
    {
        public TestRunContext(IServiceProvider defaultServiceProvider)
        {
            ServiceProvider = defaultServiceProvider;
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
