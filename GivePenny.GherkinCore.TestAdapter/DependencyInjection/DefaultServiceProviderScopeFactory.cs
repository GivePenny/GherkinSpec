using Microsoft.Extensions.DependencyInjection;

namespace GivePenny.GherkinCore.TestAdapter.DependencyInjection
{
    class DefaultServiceProviderScopeFactory : IServiceScopeFactory
    {
        public IServiceScope CreateScope()
        {
            return new DefaultServiceProviderScope();
        }
    }
}
