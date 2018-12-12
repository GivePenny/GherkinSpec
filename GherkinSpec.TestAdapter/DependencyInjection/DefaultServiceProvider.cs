using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GherkinSpec.TestAdapter.DependencyInjection
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
            => GetService(serviceType, new List<Type>());

        private object GetService(Type serviceType, List<Type> typesInStack)
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

            lock (instances)
            {
                foreach (var instance in instances)
                {
                    if (instance.GetType() == serviceType)
                    {
                        return instance;
                    }
                }

                if (typesInStack.Contains(serviceType))
                {
                    throw new InvalidOperationException(
                        $"Could not create an instance of type {serviceType.FullName} because of a circular dependency between the types of its constructor arguments.");
                }

                typesInStack.Add(serviceType);

                var newInstance = CreateInstanceOf(serviceType, typesInStack);
                instances.Add(newInstance);
                return newInstance;
            }
        }

        private object CreateInstanceOf(Type serviceType, List<Type> typesInStack)
        {
            var parameters = new List<object>();

            foreach(var parameter in MostDemandingConstructor(serviceType).GetParameters())
            {
                parameters.Add(GetService(parameter.ParameterType, typesInStack));
            }

            return Activator.CreateInstance(serviceType, parameters.ToArray());
        }

        private static ConstructorInfo MostDemandingConstructor(Type serviceType)
        {
            var highestParameterCount = 0;
            ConstructorInfo candidate = null;

            foreach (var constructor in serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                var parameterCount = constructor.GetParameters().Length;
                if (parameterCount > highestParameterCount
                    || candidate == null)
                {
                    highestParameterCount = parameterCount;
                    candidate = constructor;
                }
            }

            if (candidate == null)
            {
                throw new MissingMethodException(
                    $"No public constructor defined for the type \"{serviceType.FullName}\".");
            }

            return candidate;
        }
    }
}
