using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestAdapter.UnitTests.ReferencedAssembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GherkinSpec.TestAdapter.CustomBindingAttributesUnitTests;

[TestClass]
public class MethodScannerShould
{
    [TestMethod]
    public void FindTheExpectedMethodsWhenAnAssemblyBindingTypesAttributeIsFound()
    {
        var regularExpressionsToGivenMethods = new Dictionary<Regex, MethodInfo>();
        var regularExpressionsToWhenMethods = new Dictionary<Regex, MethodInfo>();
        var regularExpressionsToThenMethods = new Dictionary<Regex, MethodInfo>();

        var methodScanner = new MethodScanner(
            regularExpressionsToGivenMethods,
            regularExpressionsToWhenMethods,
            regularExpressionsToThenMethods);

        methodScanner.Scan(GetType().Assembly);

        var expectedCustomGivenMethod =
            regularExpressionsToGivenMethods.SingleOrDefault(kvp =>
                kvp.Value.Name == nameof(Steps.GivenSomething)
                && kvp.Value.DeclaringType == typeof(Steps));

        Assert.IsNotNull(expectedCustomGivenMethod);
        Assert.IsTrue(expectedCustomGivenMethod.Key.IsMatch("something given"));

        var expectedCustomWhenMethod =
            regularExpressionsToWhenMethods.SingleOrDefault(kvp =>
                kvp.Value.Name == nameof(Steps.WhenSomething)
                && kvp.Value.DeclaringType == typeof(Steps));

        Assert.IsNotNull(expectedCustomWhenMethod);
        Assert.IsTrue(expectedCustomWhenMethod.Key.IsMatch("something when"));

        var expectedCustomThenMethod =
            regularExpressionsToThenMethods.SingleOrDefault(kvp =>
                kvp.Value.Name == nameof(Steps.ThenSomething)
                && kvp.Value.DeclaringType == typeof(Steps));

        Assert.IsNotNull(expectedCustomThenMethod);
        Assert.IsTrue(expectedCustomThenMethod.Key.IsMatch("something then"));

        var expectedGherkinSpecGivenMethod =
            regularExpressionsToGivenMethods.SingleOrDefault(kvp =>
                kvp.Value.Name == nameof(ReferencedAssemblyStepBindingSamples.GivenAReferencedAssemblyMatch)
                && kvp.Value.DeclaringType == typeof(ReferencedAssemblyStepBindingSamples));

        Assert.IsNotNull(expectedGherkinSpecGivenMethod);
        Assert.IsTrue(expectedGherkinSpecGivenMethod.Key.IsMatch("a referenced assembly match"));
    }
}