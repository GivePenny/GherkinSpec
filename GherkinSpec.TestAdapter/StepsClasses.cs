using GherkinSpec.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GherkinSpec.TestAdapter
{
    static class StepsClasses
    {
        public static IEnumerable<Type> FindIn(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(FindIn);

        public static IEnumerable<Type> FindIn(Assembly assembly)
            => assembly.GetTypes().Where(
                type => type.IsClass
                    && type.GetCustomAttributes(
                        typeof(StepsAttribute),
                        true).Any());
    }
}
