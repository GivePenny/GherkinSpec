using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Threading.Tasks;

namespace GherkinSpec.TestAdapter.Execution
{
    public interface IStepsExecutor
    {
        Task<TestResult> Execute(TestCase testCase, DiscoveredTestData testData, TestRunContext testRunContext, IMessageLogger logger);
    }
}
