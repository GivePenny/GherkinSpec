using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Binding
{
    public interface IStepBinding
    {
        Task Execute(IServiceProvider serviceProvider, Collection<TestResultMessage> messages);

        string Name { get; }
        object[] Arguments { get; }
    }
}
