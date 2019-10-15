using GherkinSpec.TestAdapter.DependencyInjection;
using GherkinSpec.TestModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GherkinSpec.TestAdapter.UnitTests.DependencyInjection
{
    [TestClass]
    public class DefaultServiceProviderShould
    {
        [TestMethod]
        public void InstantiateClassesWithPublicParameterlessConstructors()
        {
            using var provider = new DefaultServiceProvider();
            var instance = provider.GetService(typeof(PublicParameterlessConstructor));
            Assert.IsInstanceOfType(instance, typeof(PublicParameterlessConstructor));
        }

        [TestMethod]
        public void NotInstantiateClassesWithNonPublicParameterlessConstructors()
        {
            using var provider = new DefaultServiceProvider();
            Assert.ThrowsException<MissingMethodException>(
                () => provider.GetService(typeof(ProtectedParameterlessConstructor)));
        }

        [TestMethod]
        public void InstantiateClassesWithPublicConstructorsWithParameters()
        {
            using var provider = new DefaultServiceProvider();
            var instance = provider.GetService(typeof(PublicConstructorWithParameters));
            Assert.IsInstanceOfType(instance, typeof(PublicConstructorWithParameters));
        }

        [TestMethod]
        public void AutomaticallyProvideAScopeFactory()
        {
            using var provider = new DefaultServiceProvider();
            var instance = provider.GetService(typeof(IServiceScopeFactory));
            Assert.IsInstanceOfType(instance, typeof(DefaultServiceProviderScopeFactory));
        }

        [TestMethod]
        public void AutomaticallyProvideItselfAsAServiceProvider()
        {
            using var provider = new DefaultServiceProvider();
            var instance = provider.GetService(typeof(IServiceProvider));
            Assert.AreSame(provider, instance);
        }

        [TestMethod]
        public void AutomaticallyProvideATestLogAccessor()
        {
            using var provider = new DefaultServiceProvider();
            var instance = provider.GetService(typeof(ITestLogAccessor));
            Assert.IsInstanceOfType(instance, typeof(TestLogAccessor));
        }

        [TestMethod]
        public void ReturnInstancesFromParentScopeIfInstantiatedThere()
        {
            using var outerProvider = new DefaultServiceProvider();
            var outerInstance = outerProvider.GetService(typeof(PublicParameterlessConstructor));

            var scopeFactory = (IServiceScopeFactory)outerProvider.GetService(typeof(IServiceScopeFactory));
            
            using var scope = scopeFactory.CreateScope();
            var innerInstance = scope.ServiceProvider.GetService(typeof(PublicParameterlessConstructor));
            Assert.AreSame(outerInstance, innerInstance);
        }
    }
}
