using System;

namespace GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StepsAttribute : Attribute
    {
        public StepsAttribute()
        {
        }
    }
}
