using System;

namespace GivePenny.GherkinCore.TestAdapter
{
    public class StepBindingException : Exception
    {
        public StepBindingException(string message)
            : base(message)
        {
        }
    }
}
