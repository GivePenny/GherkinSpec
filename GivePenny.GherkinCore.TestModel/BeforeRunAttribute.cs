using System;

namespace GivePenny.GherkinCore.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BeforeRunAttribute : Attribute
    {
    }
}
