using System;

namespace GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AfterRunAttribute : Attribute
    {
    }
}
