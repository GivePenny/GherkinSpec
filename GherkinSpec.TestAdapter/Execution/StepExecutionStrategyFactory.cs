using GherkinSpec.TestAdapter.Binding;

namespace GherkinSpec.TestAdapter.Execution
{
    static class StepExecutionStrategyFactory
    {
        private readonly static ExecuteOnceStepStrategy executeOnceStepStrategy = new ExecuteOnceStepStrategy();
        private readonly static EventuallySuccessfulStepStrategy eventuallySuccesfulStepStrategy = new EventuallySuccessfulStepStrategy();
        private readonly static MustNotEventuallyFailStepStrategy mustNotEventuallyFailStepStrategy = new MustNotEventuallyFailStepStrategy();

        public static IStepExecutionStrategy GetFor(IStepBinding binding)
        {
            if (binding.IsSuccessEventual)
            {
                return eventuallySuccesfulStepStrategy;
            }

            if (binding.IsMarkedMustNotEventuallyFail)
            {
                return mustNotEventuallyFailStepStrategy;
            }

            return executeOnceStepStrategy;
        }
    }
}