using Microsoft.Extensions.DependencyInjection;

namespace GherkinSpec.TestAdapter.DependencyInjection
{
    internal class DefaultServiceProviderScopeFactory : IServiceScopeFactory
    {
        private readonly DefaultServiceProvider outerScopeProvider;

        public DefaultServiceProviderScopeFactory(DefaultServiceProvider outerScopeProvider)
        {
            this.outerScopeProvider = outerScopeProvider;
        }

        public IServiceScope CreateScope()
        {
            return new DefaultServiceProviderScope(outerScopeProvider);
        }
    }
}
