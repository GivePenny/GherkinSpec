﻿using Microsoft.Extensions.DependencyInjection;
using System;

namespace GherkinSpec.TestAdapter.DependencyInjection
{
    class DefaultServiceProviderScope : IServiceScope
    {
        public DefaultServiceProviderScope()
        {
            ServiceProvider = new DefaultServiceProvider();
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            ((DefaultServiceProvider)ServiceProvider).Dispose();
        }
    }
}
