using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GivePenny.GherkinSpec.TestAdapter
{
    public interface IMethodMapping
    {
        Task Execute(IServiceProvider serviceProvider, Collection<TestResultMessage> messages);

        string Name { get; }
        object[] Arguments { get; }
    }
}
