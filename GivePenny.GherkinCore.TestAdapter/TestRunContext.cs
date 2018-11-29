using System;

namespace GivePenny.GherkinCore.TestAdapter
{
    public delegate object ObjectFactory(Type typeToInstantiate);

    public class TestRunContext
    {
        public TestRunContext(IServiceProvider defaultServiceProvider)
        {
            ServiceProvider = defaultServiceProvider;
        }

        public IServiceProvider ServiceProvider { get; set; }
    }
}
