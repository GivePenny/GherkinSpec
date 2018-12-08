using System;

namespace GherkinSpec.TestModel
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
