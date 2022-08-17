using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Execution
{
    internal interface IStepExecutionStrategy
    {
        Task Execute(IStepBinding stepBinding, IServiceProvider serviceProvider, Collection<TestResultMessage> messages, TestRunContext testRunContext);
    }
}
