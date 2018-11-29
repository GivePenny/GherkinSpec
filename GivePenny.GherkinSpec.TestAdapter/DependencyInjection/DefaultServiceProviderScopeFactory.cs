using Microsoft.Extensions.DependencyInjection;

namespace GivePenny.GherkinSpec.TestAdapter.DependencyInjection
{
    class DefaultServiceProviderScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return new DefaultServiceProviderScope();
        }
    }
}
