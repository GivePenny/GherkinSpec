using GivePenny.GherkinSpec.Model;
using GivePenny.GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GivePenny.GherkinSpec.TestAdapter
{
    class MethodMapper : IMethodMapper
    {
        public IMethodMapping GetMappingFor(IStep step, Assembly testAssembly)
        {
            // TODO Cache list of methods and regexs, and separate list of steps to methods
            // TODO Referenced assemblies

            MethodMapping candidate = null;

            foreach(var type in StepsClasses.FindIn(testAssembly))
            {
                foreach (var method in FindMatchingStepMethodsIn(type, step))
                {
                    if (candidate != null)
                    {
                        // TODO Record details
                        throw new StepBindingException(
                            $"Found more than one step definitions matching the step '{step.Title}'.");
                    }

                    candidate = method;
                }
            }

            if (candidate == null)
            {
                throw new StepBindingException(
                    $"Could not find a step definition to match the step '{step.Title}'.");
            }

            return candidate;
        }

        private static IEnumerable<MethodMapping> FindMatchingStepMethodsIn(Type type, IStep step)
        {
            foreach(var method in type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                foreach (var match in GetMatches<GivenStep, GivenAttribute>(method, step)
                    .Concat(GetMatches<WhenStep, WhenAttribute>(method, step))
                    .Concat(GetMatches<ThenStep, ThenAttribute>(method, step)))
                {
                    yield return match;
                }
            }
        }

        private static IEnumerable<MethodMapping> GetMatches<TStep, TAttribute>(MethodInfo method, IStep step)
            where TStep : IStep
            where TAttribute : Attribute, IStepAttribute
        {
            if (!(step is TStep))
            {
                yield break;
            }

            var attributes = method.GetCustomAttributes<TAttribute>(true);
            foreach (var attribute in attributes)
            {
                // TODO Handle regex syntax exceptioms
                var regex = new Regex(
                    attribute.MatchExpression,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

                var match = regex.Match(step.TitleAfterType);
                if (match != null && match.Success)
                {
                    var arguments = ParseArgumentValues(
                        step: step,
                        stepTitleExtracts: match.Groups.Skip(1).Select(group => group.Value).ToArray(),
                        method: method);
                    yield return new MethodMapping(step, method, arguments);
                }
            }
        }

        private static object[] ParseArgumentValues(IStep step, string[] stepTitleExtracts, MethodInfo method)
        {
            var typedValues = new List<object>();
            var parameters = method.GetParameters();

            if (parameters.Length > stepTitleExtracts.Length)
            {
                throw new StepBindingException(
                    $"The step \"{step.Title}\" matches the method \"{method.Name}\" on class \"{method.DeclaringType.FullName}\". That method expects {parameters.Length} parameters but the step only provides {stepTitleExtracts.Length}.");
            }

            var index = 0;
            foreach (var parameter in parameters)
            {
                typedValues.Add(ConvertValue(stepTitleExtracts[index], parameter.ParameterType));
                index++;
            }

            return typedValues.ToArray();
        }

        private static object ConvertValue(string value, Type targetType)
            => TypeDescriptor
                .GetConverter(targetType)
                .ConvertFromString(value);
    }
}
