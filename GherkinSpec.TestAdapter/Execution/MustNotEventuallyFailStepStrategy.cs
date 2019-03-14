using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace GherkinSpec.TestAdapter.Execution
{
    class MustNotEventuallyFailStepStrategy : IStepExecutionStrategy
    {
        public async Task Execute(IStepBinding stepBinding, IServiceProvider serviceProvider, Collection<TestResultMessage> messages, TestRunContext testRunContext)
        {
            var attempts = 0;
            do
            {
                attempts++;

                await stepBinding
                    .Execute(serviceProvider, messages)
                    .ConfigureAwait(false);

                if (attempts >= testRunContext.EventualSuccess.MaximumAttempts)
                {
                    break;
                }

                messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{StepsExecutor.StepLogIndent}Passed at {DateTime.UtcNow:o}, waiting and checking again{Environment.NewLine}"));

                await Task
                    .Delay(testRunContext.EventualSuccess.DelayBetweenAttempts)
                    .ConfigureAwait(false);
            } while (true);
        }
    }
}
