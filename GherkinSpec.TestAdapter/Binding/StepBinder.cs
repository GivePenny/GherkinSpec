using GherkinSpec.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GherkinSpec.TestAdapter.Binding
{
    internal class StepBinder : IStepBinder
    {
        private readonly List<Assembly> scannedAssemblies = new();
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToGivenMethods = new();
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToWhenMethods = new();
        private readonly Dictionary<Regex, MethodInfo> regularExpressionsToThenMethods = new();
        private readonly ConcurrentDictionary<IStep, IStepBinding> cachedStepBindings = new();

        public IStepBinding GetBindingFor(IStep step, Assembly testAssembly)
            => cachedStepBindings.GetOrAdd(
                step,
                keyStep => GetBindingForStepWithoutCache(keyStep, testAssembly));

        private IStepBinding GetBindingForStepWithoutCache(IStep step, Assembly testAssembly)
        {
            EnsureHasBeenScanned(testAssembly);

            if (step is GivenStep)
            {
                return GetBindingFor(step, regularExpressionsToGivenMethods);
            }

            if (step is WhenStep)
            {
                return GetBindingFor(step, regularExpressionsToWhenMethods);
            }

            if (step is ThenStep)
            {
                return GetBindingFor(step, regularExpressionsToThenMethods);
            }

            throw new NotSupportedException(
                $"Unsupported attribute type \"{step.GetType().FullName}\".");
        }

        private void EnsureHasBeenScanned(Assembly testAssembly)
        {
            if (scannedAssemblies.Contains(testAssembly))
            {
                return;
            }

            lock (scannedAssemblies)
            {
                if (scannedAssemblies.Contains(testAssembly))
                {
                    return;
                }

                var scanner = new MethodScanner(regularExpressionsToGivenMethods, regularExpressionsToWhenMethods, regularExpressionsToThenMethods);
                scanner.Scan(testAssembly);
                scannedAssemblies.Add(testAssembly);
            }
        }

        private IStepBinding GetBindingFor(IStep step, Dictionary<Regex, MethodInfo> regularExpressionsToMethods)
        {
            StepBinding candidate = null;

            foreach (var mapping in FindMatchingStepMethodsIn(step, regularExpressionsToMethods))
            {
                if (candidate != null)
                {
                    throw new StepBindingException(
                        $"Found more than one step definitions matching the step '{step.Title}' of type '{step.GetType().Name}'. One was '{candidate.FullName}' and another was '{mapping.FullName}'. There may be more but the search was stopped here.");
                }

                candidate = mapping;
            }

            if (candidate == null)
            {
                throw new StepBindingException(
                    $"Could not find a step definition to match the step '{step.Title}' (step type is a '{step.GetType().Name}').");
            }

            return candidate;
        }

        private static IEnumerable<StepBinding> FindMatchingStepMethodsIn(IStep step, Dictionary<Regex, MethodInfo> regularExpressionsToMethods)
        {
            foreach (var regexMethodPair in regularExpressionsToMethods)
            {
                var match = regexMethodPair.Key.Match(step.TitleAfterType);
                if (match.Success)
                {
                    var stepArguments = match
                        .Groups
                        .Skip(1)
                        .Select(group => group.Value);

                    if (step.MultiLineStringArgument != null)
                    {
                        stepArguments = stepArguments.Append(step.MultiLineStringArgument);
                    }

                    var arguments = ParseArgumentValues(
                        step: step,
                        stepArguments: stepArguments.ToArray(),
                        method: regexMethodPair.Value);
                    yield return new StepBinding(step, regexMethodPair.Value, arguments);
                }
            }
        }

        private static object[] ParseArgumentValues(IStep step, string[] stepArguments, MethodInfo method)
        {
            var typedValues = new List<object>();
            var parameters = method.GetParameters();

            var methodHasTableArgument = parameters.Length > 0
                && parameters.Last().ParameterType == typeof(DataTable);

            ThrowIfExpectsTableButNoneProvided(step, method, methodHasTableArgument);
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

        private static void ThrowIfExpectsTableButNoneProvided(IStep step, MethodInfo method, bool methodHasTableArgument)
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
                .ConvertFromString(null, CultureInfo.CurrentUICulture, value);
    }
}
