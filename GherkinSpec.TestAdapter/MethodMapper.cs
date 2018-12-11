using GherkinSpec.Model;
using GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter
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
                    var stepArguments = match.Groups.Skip(1).Select(group => group.Value);

                    if(step.MultiLineStringArgument!=null)
                    {
                        stepArguments = stepArguments.Append(step.MultiLineStringArgument);
                    }

                    var arguments = ParseArgumentValues(
                        step: step,
                        stepArguments: stepArguments.ToArray(),
                        method: method);
                    yield return new MethodMapping(step, method, arguments);
                }
            }
        }

        private static object[] ParseArgumentValues(IStep step, string[] stepArguments, MethodInfo method)
        {
            var typedValues = new List<object>();
            var parameters = method.GetParameters();

            var methodHasTableArgument = parameters.Length > 0
                && parameters.Last().ParameterType == typeof(DataTable);

            ThrowIfExpectsTableButNoneProvided(step, method, parameters, methodHasTableArgument);
            ThrowIfTableProvidedButNoneExpected(step, method, methodHasTableArgument);
            ThrowIfNotEnoughParametersProvided(step, stepArguments, method, parameters, methodHasTableArgument);

            var index = 0;
            foreach (var parameter in parameters)
            {
                if (methodHasTableArgument && parameter.ParameterType == typeof(DataTable))
                {
                    typedValues.Add(step.TableArgument);
                    continue;
                }

                typedValues.Add(
                    ConvertValue(stepArguments[index], parameter.ParameterType));
                index++;
            }

            return typedValues.ToArray();
        }

        private static void ThrowIfExpectsTableButNoneProvided(IStep step, MethodInfo method, ParameterInfo[] parameters, bool methodHasTableArgument)
        {
            if (methodHasTableArgument && step.TableArgument.IsEmpty)
            {
                throw new StepBindingException(
                    $"The step \"{step.Title}\" matches the method \"{method.Name}\" on class \"{method.DeclaringType.FullName}\". That method expects a table argument but the step does not provide one.");
            }
        }

        private static void ThrowIfNotEnoughParametersProvided(IStep step, string[] stepArguments, MethodInfo method, ParameterInfo[] parameters, bool methodHasTableArgument)
        {
            if (parameters.Length > (stepArguments.Length + (methodHasTableArgument ? 1 : 0)))
            {
                throw new StepBindingException(
                    $"The step \"{step.Title}\" matches the method \"{method.Name}\" on class \"{method.DeclaringType.FullName}\". That method expects {parameters.Length} parameters but the step only provides {stepArguments.Length}.");
            }
        }

        private static void ThrowIfTableProvidedButNoneExpected(IStep step, MethodInfo method, bool methodHasTableArgument)
        {
            if (!step.TableArgument.IsEmpty && !methodHasTableArgument)
            {
                throw new StepBindingException(
                    $"The step \"{step.Title}\" matches the method \"{method.Name}\" on class \"{method.DeclaringType.FullName}\". That method does not expect a table argument but the step provides one.");
            }
        }

        private static object ConvertValue(string value, Type targetType)
            => TypeDescriptor
                .GetConverter(targetType)
                .ConvertFromString(value);
    }
}
