using GherkinSpec.TestModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GherkinSpec.TestAdapter.DependencyInjection
{
    internal class DefaultServiceProvider : IServiceProvider, IDisposable
    {
        private readonly List<object> instances = new List<object>();
        private readonly DefaultServiceProvider outerScopeProvider;

        public DefaultServiceProvider()
            : this(null)
        {
        }

        public DefaultServiceProvider(DefaultServiceProvider outerScopeProvider)
        {
            this.outerScopeProvider = outerScopeProvider;
            instances.Add(this);
        }

        public void Dispose()
        {
            lock (instances)
            {
                foreach (var instance in instances
                    .OfType<IDisposable>()
                    .Where(it => it != this))
                {
                    instance.Dispose();
                }
            }
        }

        public object GetService(Type serviceType)
            => GetService(
                serviceType,
                new List<Type>(),
                instantiateIfNotFound: true,
                permitOuterScopeSearch: true);

        public T GetService<T>()
            => (T)GetService(typeof(T));

        private object GetService(Type serviceType, List<Type> typesInStack, bool instantiateIfNotFound, bool permitOuterScopeSearch)
        {
            var builtInResult = TryGetBuiltInService(serviceType, typesInStack, instantiateIfNotFound, permitOuterScopeSearch);
            if (builtInResult != null)
            {
                return builtInResult;
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

                if (outerScopeProvider != null && permitOuterScopeSearch)
                {
                    var outerInstance = outerScopeProvider.GetService(
                        serviceType,
                        typesInStack,
                        instantiateIfNotFound: false,
                        permitOuterScopeSearch: true);

                    if (outerInstance != null)
                    {
                        return outerInstance;
                    }
                }

                if (!instantiateIfNotFound)
                {
                    return null;
                }

                typesInStack.Add(serviceType);

                var newInstance = CreateInstanceOf(serviceType, typesInStack);
                instances.Add(newInstance);
                return newInstance;
            }
        }

        private object TryGetBuiltInService(Type serviceType, List<Type> typesInStack, bool instantiateIfNotFound, bool permitOuterScopeSearch)
        {
            if (serviceType == typeof(IServiceScopeFactory))
            {
                return GetService(
                    typeof(DefaultServiceProviderScopeFactory),
                    typesInStack,
                    instantiateIfNotFound,
                    permitOuterScopeSearch: false);
            }

            if (serviceType == typeof(ITestLogAccessor))
            {
                return GetService(typeof(TestLogAccessor), typesInStack, instantiateIfNotFound, permitOuterScopeSearch);
            }

            if (serviceType == typeof(IServiceProvider))
            {
                return GetService(typeof(DefaultServiceProvider), typesInStack, instantiateIfNotFound, permitOuterScopeSearch);
            }

            return null;
        }

        private object CreateInstanceOf(Type serviceType, List<Type> typesInStack)
        {
            var parameters = new List<object>();

            foreach (var parameter in MostDemandingConstructor(serviceType).GetParameters())
            {
                parameters.Add(
                    GetService(
                        parameter.ParameterType,
                        typesInStack,
                        instantiateIfNotFound: true,
                        permitOuterScopeSearch: true));
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
