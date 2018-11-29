using System;

namespace GivePenny.GherkinSpec.TestAdapter
{
    public class StepBindingException : Exception
    {
        public StepBindingException(string message)
            : base(message)
        {
        }
    }
}
