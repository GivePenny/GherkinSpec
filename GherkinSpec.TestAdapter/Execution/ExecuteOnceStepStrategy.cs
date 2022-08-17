using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace GherkinSpec.TestAdapter.Execution
{
    internal class ExecuteOnceStepStrategy : IStepExecutionStrategy
    {
        public Task Execute(IStepBinding stepBinding, IServiceProvider serviceProvider, Collection<TestResultMessage> messages, TestRunContext testRunContext)
            => stepBinding
                .Execute(serviceProvider, messages);
    }
}
