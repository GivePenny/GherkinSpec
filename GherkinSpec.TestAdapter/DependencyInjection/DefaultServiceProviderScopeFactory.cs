using Microsoft.Extensions.DependencyInjection;

namespace GherkinSpec.TestAdapter.DependencyInjection
{
    class DefaultServiceProviderScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return new DefaultServiceProviderScope();
        }
    }
}
