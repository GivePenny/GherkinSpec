using System;

namespace GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MustNotEventuallyFailAttribute : Attribute
    {
    }
}
