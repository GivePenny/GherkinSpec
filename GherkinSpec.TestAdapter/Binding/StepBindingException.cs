using System;

namespace GherkinSpec.TestAdapter.Binding
{
    public class StepBindingException : Exception
    {
        public StepBindingException(string message)
            : base(message)
        {
        }
    }
}
