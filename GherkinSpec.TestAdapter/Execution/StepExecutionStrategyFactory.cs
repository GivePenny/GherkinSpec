using GherkinSpec.TestAdapter.Binding;

namespace GherkinSpec.TestAdapter.Execution
{
    internal static class StepExecutionStrategyFactory
    {
        private static readonly ExecuteOnceStepStrategy ExecuteOnceStepStrategy = new();
        private static readonly EventuallySuccessfulStepStrategy EventuallySuccessfulStepStrategy = new();
        private static readonly MustNotEventuallyFailStepStrategy MustNotEventuallyFailStepStrategy = new();

        public static IStepExecutionStrategy GetFor(IStepBinding binding)
        {
            if (binding.IsSuccessEventual)
            {
                return EventuallySuccessfulStepStrategy;
            }

            if (binding.IsMarkedMustNotEventuallyFail)
            {
                return MustNotEventuallyFailStepStrategy;
            }

            return ExecuteOnceStepStrategy;
        }
    }
}