using Microsoft.Extensions.DependencyInjection;
using System;

namespace GherkinSpec.TestAdapter.DependencyInjection
{
    internal class DefaultServiceProviderScope : IServiceScope
    {
        public DefaultServiceProviderScope(DefaultServiceProvider outerScopeProvider)
        {
            ServiceProvider = new DefaultServiceProvider(outerScopeProvider);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            ((DefaultServiceProvider)ServiceProvider).Dispose();
        }
    }
}
