﻿using System;

namespace GherkinSpec.TestModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WhenAttribute : Attribute, IStepAttribute
    {
        public WhenAttribute(string matchExpression)
        {
            MatchExpression = "^" + matchExpression + "$";
        }

        public string MatchExpression { get; }
    }
}
