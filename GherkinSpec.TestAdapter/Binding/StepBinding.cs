using GherkinSpec.Model;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Binding
{
    class StepBinding : IStepBinding
    {
        private readonly IStep step;
        private readonly MethodInfo methodInfo;
        private readonly object[] arguments;

        public StepBinding(IStep step, MethodInfo methodInfo, object[] arguments)
        {
            this.step = step;
            this.methodInfo = methodInfo;
            this.arguments = arguments;
        }

        public string Name => methodInfo.Name;

        public string FullName => methodInfo.DeclaringType.FullName + "::" + methodInfo.Name;

        public object[] Arguments => arguments;

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

            var result = methodInfo.Invoke(stepsClassInstance, arguments);

            if (methodInfo.ReturnType.IsSubclassOf(typeof(Task)) || methodInfo.ReturnType == typeof(Task))
            {
                return (Task)result;
            }

            return Task.CompletedTask;
        }
    }
}
