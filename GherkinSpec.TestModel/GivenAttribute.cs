using System;

namespace GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class GivenAttribute : Attribute, IStepAttribute
    {
        public GivenAttribute(string matchExpression)
        {
            MatchExpression = "^" + matchExpression + "$";
        }

        public string MatchExpression { get; }
    }
}
