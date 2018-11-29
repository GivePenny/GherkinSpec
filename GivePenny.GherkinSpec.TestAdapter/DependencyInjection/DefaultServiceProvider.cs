using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace GivePenny.GherkinSpec.TestAdapter.DependencyInjection
{
    class DefaultServiceProvider : IServiceProvider, IDisposable
    {
        private readonly List<object> instances = new List<object>();

        public void Dispose()
        {
            lock (instances)
            {
                foreach (var instance in instances)
                {
                    var disposable = instance as IDisposable;
                    disposable?.Dispose();
                }
            }
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceScopeFactory))
            {
                return GetService(typeof(DefaultServiceProviderScopeFactory));
            }

            if (serviceType.IsInterface)
            {
                throw new InvalidOperationException(
                    $"The default service provider cannot resolve interfaces into a concrete type. Configure a different service provider and register a concrete type to fulfil the \"{serviceType.FullName}\" service.");
            }

            // TODO Thread safety (and performance) review
            lock (instances)
            {
                foreach (var instance in instances)
                {
                    if (instance.GetType() == serviceType)
                    {
                        return instance;
                    }
                }

                var newInstance = CreateInstanceOf(serviceType);
                instances.Add(newInstance);
                return newInstance;
            }
        }

        private object CreateInstanceOf(Type serviceType)
        {
            // TODO Support constructor arguments as long as they are simple objects - block circular refs
            return Activator.CreateInstance(serviceType);
        }
    }
}
