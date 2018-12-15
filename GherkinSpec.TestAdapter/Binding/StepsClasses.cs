using GherkinSpec.TestModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GherkinSpec.TestAdapter.Binding
{
    static class StepsClasses
    {
        // Test runners use an isolated appdomain so the cache can be static as it will be discarded after each test run or when the assembly
        // is changed and re-built.
        private static readonly ConcurrentDictionary<Assembly, Type[]> cachedTypesByAssembly = new ConcurrentDictionary<Assembly, Type[]>();

        public static IEnumerable<Type> FindIn(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(FindIn);

        public static IEnumerable<Type> FindIn(Assembly assembly)
            => cachedTypesByAssembly.GetOrAdd(
                assembly,
                keyAssembly => keyAssembly
                    .GetTypes()
                    .Where(
                        type => type.IsClass
                            && type.IsPublic
                            && type.GetCustomAttributes(
                                typeof(StepsAttribute),
                                true).Any())
                    .ToArray());
    }
}
