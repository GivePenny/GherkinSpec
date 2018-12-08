using System;

namespace GherkinSpec.TestAdapter
{
    public class StepBindingException : Exception
    {
        public StepBindingException(string message)
            : base(message)
        {
        }
    }
}
