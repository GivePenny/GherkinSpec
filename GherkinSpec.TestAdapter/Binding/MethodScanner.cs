using GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

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

        public void Scan(Assembly stepsAssembly)
        {
            foreach (var type in StepsClasses.FindIn(stepsAssembly))
            {
                Scan(type);
            }
        }

        private void Scan(Type type)
        {
            foreach (var method in type.GetMethods(
                BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                Scan<GivenAttribute>(method, regularExpressionsToGivenMethods);
                Scan<WhenAttribute>(method, regularExpressionsToWhenMethods);
                Scan<ThenAttribute>(method, regularExpressionsToThenMethods);
            }
        }

        private void Scan<TAttribute>(MethodInfo method, Dictionary<Regex, MethodInfo> regularExpressionsToMethods)
            where TAttribute : Attribute, IStepAttribute
        {
            var attributes = method.GetCustomAttributes<TAttribute>(true);
            foreach (var attribute in attributes)
            {
                var regex = GetRegex(
                    method.DeclaringType.FullName + "::" + method.Name,
                    attribute.MatchExpression);

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
