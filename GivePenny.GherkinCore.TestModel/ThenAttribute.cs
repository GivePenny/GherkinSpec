using System;

namespace GivePenny.GherkinCore.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ThenAttribute : Attribute, IStepAttribute
    {
        public ThenAttribute(string matchExpression)
        {
            MatchExpression = "^" + matchExpression + "$";
        }

        public string MatchExpression { get; }
    }
}
