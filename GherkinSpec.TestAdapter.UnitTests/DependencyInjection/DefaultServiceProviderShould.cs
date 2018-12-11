using GherkinSpec.TestAdapter.DependencyInjection;
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
            using (var provider = new DefaultServiceProvider())
            {
                var instance = provider.GetService(typeof(PublicParameterlessConstructor));
                Assert.IsInstanceOfType(instance, typeof(PublicParameterlessConstructor));
            }
        }

        [TestMethod]
        public void NotInstantiateClassesWithNonPublicParameterlessConstructors()
        {
            using (var provider = new DefaultServiceProvider())
            {
                Assert.ThrowsException<MissingMethodException>(
                    () => provider.GetService(typeof(ProtectedParameterlessConstructor)));
            }
        }

        [TestMethod]
        public void InstantiateClassesWithPublicConstructorsWithParameters()
        {
            using (var provider = new DefaultServiceProvider())
            {
                var instance = provider.GetService(typeof(PublicConstructorWithParameters));
                Assert.IsInstanceOfType(instance, typeof(PublicConstructorWithParameters));
            }
        }
    }
}
