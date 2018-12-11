namespace GherkinSpec.TestAdapter.UnitTests.DependencyInjection
{
    class PublicConstructorWithParameters
    {
        public PublicConstructorWithParameters(PublicParameterlessConstructor dependency, PublicParameterlessConstructor anotherDependency)
        {
        }
    }
}
