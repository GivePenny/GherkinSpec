using GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GherkinSpec.TestModel;

namespace GherkinSpec.TestAdapter.Binding
{
    class MethodScanner
    {
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToGivenMethods;
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToWhenMethods;
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToThenMethods;

        public MethodScanner(
            Dictionary<Regex, MethodInfo> regularExpressionsToGivenMethods,
            Dictionary<Regex, MethodInfo> regularExpressionsToWhenMethods,
            Dictionary<Regex, MethodInfo> regularExpressionsToThenMethods)
        {
            this.regularExpressionsToGivenMethods = regularExpressionsToGivenMethods;
            this.regularExpressionsToWhenMethods = regularExpressionsToWhenMethods;
            this.regularExpressionsToThenMethods = regularExpressionsToThenMethods;
        }

        public void Scan(Assembly testAssembly)
        {
            foreach (var stepsClassesAndBindingTypes in StepsClasses.FindInAssemblyAndReferencedAssemblies(testAssembly))
            {
                Scan(stepsClassesAndBindingTypes);
            }
        }

        private void Scan((IEnumerable<Type> Types, BindingTypesAttribute BindingTypes) stepsClassesAndBindingType)
        {
            foreach (var method in stepsClassesAndBindingType.Types.SelectMany(t => t.GetMethods(
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)))
            {
                Scan(
                    stepsClassesAndBindingType.BindingTypes.GivenStepAttributeType,
                    stepsClassesAndBindingType.BindingTypes.StepAttributeMatchExpressionSelectorType,
                    method,
                    regularExpressionsToGivenMethods);
                
                Scan(
                    stepsClassesAndBindingType.BindingTypes.WhenStepAttributeType,
                    stepsClassesAndBindingType.BindingTypes.StepAttributeMatchExpressionSelectorType,
                    method,
                    regularExpressionsToWhenMethods);
                
                Scan(
                    stepsClassesAndBindingType.BindingTypes.ThenStepAttributeType,
                    stepsClassesAndBindingType.BindingTypes.StepAttributeMatchExpressionSelectorType,
                    method,
                    regularExpressionsToThenMethods);
            }
        }

        private void Scan(
            Type attributeType,
            Type stepAttributeMatchExpressionSelectorType,
            MethodInfo method,
            Dictionary<Regex, MethodInfo> regularExpressionsToMethods)
        {
            var stepAttributeMatchExpressionSelector = (IStepAttributeMatchExpressionSelector)Activator
                .CreateInstance(stepAttributeMatchExpressionSelectorType);
            
            var attributes = method.GetCustomAttributes(attributeType, true);
            foreach (var attribute in attributes)
            {
                var regex = GetRegex(
                    method.DeclaringType.FullName + "::" + method.Name,
                    stepAttributeMatchExpressionSelector.SelectMatchExpression(attribute));

                regularExpressionsToMethods.Add(regex, method);
            }
        }

        private static Regex GetRegex(string methodFullName, string expression)
        {
            try
            {
                return new Regex(
                    expression,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(
                    $"Method \"{methodFullName}\" has an invalid regular expression in an attribute. The invalid expression is: {expression}", exception);
            }
        }
    }
}
