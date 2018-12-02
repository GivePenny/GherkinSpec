using GivePenny.GherkinSpec.TestModel;

namespace GivePenny.GherkinSpec.TestAdapter.UnitTests.Samples
{ 
    [Steps]
    public class HookSamples
    {
        public static int BeforeRunCallCount { get; private set; }
        public static int AfterRunCallCount { get; private set; }

        [BeforeRun]
        public static void Setup(TestRunContext testRunContext)
        {
            BeforeRunCallCount++;
        }

        [AfterRun]
        public static void Teardown(TestRunContext testRunContext)
        {
            AfterRunCallCount++;
        }
    }
}
