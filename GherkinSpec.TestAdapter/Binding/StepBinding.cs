using GherkinSpec.Model;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Binding
{
    internal class StepBinding : IStepBinding
    {
        private readonly IStep step;
        private readonly MethodInfo methodInfo;

        public StepBinding(IStep step, MethodInfo methodInfo, object[] arguments)
        {
            this.step = step;
            this.methodInfo = methodInfo;
            Arguments = arguments;
        }

        public string Name => methodInfo.Name;

        public string FullName => methodInfo.DeclaringType.FullName + "::" + methodInfo.Name;

        public object[] Arguments { get; }

        public bool IsSuccessEventual
            => methodInfo.GetCustomAttributes<EventuallySucceedsAttribute>().Any();

        public Task Execute(IServiceProvider serviceProvider, Collection<TestResultMessage> messages)
        {
            messages.Add(
                new TestResultMessage(
                    TestResultMessage.DebugTraceCategory,
                    $"Invoking \"{methodInfo.Name}\" in \"{methodInfo.DeclaringType.FullName}\"{Environment.NewLine}"));

            object stepsClassInstance = null;
            if (!methodInfo.IsStatic)
            {
                stepsClassInstance = serviceProvider.GetService(methodInfo.DeclaringType);
                if (stepsClassInstance == null)
                {
                    throw new StepBindingException(
                        $"Step \"{step.Title}\" uses method \"{methodInfo.Name}\" on class \"{methodInfo.DeclaringType.FullName}\" but that class has not been registered with the service provider.");
                }
            }

            try
            {
                var result = methodInfo.Invoke(stepsClassInstance, Arguments);

                if (methodInfo.ReturnType.IsSubclassOf(typeof(Task)) || methodInfo.ReturnType == typeof(Task))
                {
                    return (Task)result;
                }
            }
            catch (TargetInvocationException exception)
            {
                ExceptionDispatchInfo
                    .Capture(exception.InnerException)
                    .Throw();
            }

            return Task.CompletedTask;
        }
    }
}
