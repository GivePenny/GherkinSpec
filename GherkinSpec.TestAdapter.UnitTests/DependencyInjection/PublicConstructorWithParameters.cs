namespace GherkinSpec.TestAdapter.UnitTests.DependencyInjection
{
    internal class PublicConstructorWithParameters
    {
        public PublicConstructorWithParameters(PublicParameterlessConstructor dependency, PublicParameterlessConstructor anotherDependency)
        {
        }
    }
}
