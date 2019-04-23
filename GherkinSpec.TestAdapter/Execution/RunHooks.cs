using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Execution
{
    internal class RunHooks
    {
        private readonly TestRunContext testRunContext;
        private readonly IEnumerable<Assembly> assembliesToScan;

        public RunHooks(TestRunContext testRunContext, IEnumerable<Assembly> assembliesToScan)
        {
            this.testRunContext = testRunContext;
            this.assembliesToScan = assembliesToScan;
        }

        public async Task ExecuteBeforeRun()
        {
            foreach (var stepsClass in StepsClasses.FindInAssemblyAndReferencedAssemblies(assembliesToScan))
            {
                foreach (var method in FindMethodsWithAttribute<BeforeRunAttribute>(stepsClass))
                {
                    await ExecuteMethod(stepsClass, method)
                        .ConfigureAwait(false);
                }
            }
        }

        private static void ThrowHookBindingException(string attributeName, Type stepsClass, MethodInfo method)
        {
            throw new HookBindingException(
                $"Method \"{method.Name}\" in class \"{stepsClass.FullName}\" is marked with a {attributeName}. Methods marked with that attribute must be static and must accept a single argument of type {nameof(TestRunContext)}.");
        }

        public async Task ExecuteAfterRun()
        {
            foreach (var stepsClass in StepsClasses.FindInAssemblyAndReferencedAssemblies(assembliesToScan))
            {
                foreach (var method in FindMethodsWithAttribute<AfterRunAttribute>(stepsClass))
                {
                    await ExecuteMethod(stepsClass, method)
                        .ConfigureAwait(false);
                }
            }
        }

        private IEnumerable<MethodInfo> FindMethodsWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
            => type
                .GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes<TAttribute>(true).Any());

        private async Task ExecuteMethod(Type type, MethodInfo method)
        {
            var parameters = method.GetParameters();

            if (!method.IsStatic
                || parameters.Length != 1
                || parameters.First().ParameterType != typeof(TestRunContext))
            {
                ThrowHookBindingException(nameof(AfterRunAttribute), type, method);
            }

            var result = method.Invoke(null, new object[] { testRunContext });

            if (method.ReturnType.IsSubclassOf(typeof(Task)) || method.ReturnType == typeof(Task))
            {
                await ((Task)result)
                    .ConfigureAwait(false);
            }
        }
    }
}
