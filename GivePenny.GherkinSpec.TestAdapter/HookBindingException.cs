using System;

namespace GivePenny.GherkinSpec.TestAdapter
{
    public class HookBindingException : Exception
    {
        public HookBindingException(string message)
            : base(message)
        {
        }
    }
}
