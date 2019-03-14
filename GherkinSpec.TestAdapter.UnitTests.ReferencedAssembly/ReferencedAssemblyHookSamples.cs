using GherkinSpec.TestModel;

namespace GherkinSpec.TestAdapter.UnitTests.ReferencedAssembly
{
    [Steps]
    public class ReferencedAssemblyHookSamples
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
