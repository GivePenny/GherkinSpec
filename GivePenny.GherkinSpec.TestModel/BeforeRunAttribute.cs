using System;

namespace GivePenny.GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BeforeRunAttribute : Attribute
    {
    }
}
