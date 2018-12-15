using System;

namespace GherkinSpec.TestAdapter.Binding
{
    public class HookBindingException : Exception
    {
        public HookBindingException(string message)
            : base(message)
        {
        }
    }
}
