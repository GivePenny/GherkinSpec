using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GherkinSpec.TestAdapter.Binding;
using GherkinSpec.TestModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace GherkinSpec.TestAdapter.Execution
{
    class EventuallySuccessfulStepStrategy : IStepExecutionStrategy
    {
        public async Task Execute(IStepBinding stepBinding, IServiceProvider serviceProvider, Collection<TestResultMessage> messages, TestRunContext testRunContext)
        {
            var attempts = 0;
            do
            {
                attempts++;

                try
                {
                    await stepBinding
                        .Execute(serviceProvider, messages)
                        .ConfigureAwait(false);

                    break;
                }
                catch (Exception exception)
                {
                    if (IsTerminal(exception, stepBinding, attempts, testRunContext))
                    {
                        throw;
                    }
                }

                messages.Add(
                    new TestResultMessage(
                        TestResultMessage.StandardOutCategory,
                        $"{StepsExecutor.StepLogIndent}Failed, waiting and trying again{Environment.NewLine}"));

                await Task
                    .Delay(testRunContext.EventualSuccess.DelayBetweenAttempts)
                    .ConfigureAwait(false);

            } while (true);
        }

        private static bool IsTerminal(Exception exception, IStepBinding stepBinding, int attempts, TestRunContext testRunContext)
            => exception is StepBindingException
                || stepBinding == null
                || !stepBinding.IsSuccessEventual
                || attempts >= testRunContext.EventualSuccess.MaximumAttempts;
    }
}
