﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GherkinSpec.TestModel
{
    public static class StepsClasses
    {
        // Test runners use an isolated appdomain so the cache can be static as it will be discarded after each test run or when the assembly
        // is changed and re-built.
        private static readonly ConcurrentDictionary<Assembly, Type[]> cachedTypesByAssembly = new ConcurrentDictionary<Assembly, Type[]>();

        public static IEnumerable<Type> FindInAssemblyAndReferencedAssemblies(Assembly assembly)
            => assembly
                .GetReferencedAssemblies()
                .Select(
                    a => 
                        Assembly.Load(a.FullName))
                .SelectMany(FindIn)
                .Concat(FindIn(assembly));

        public static IEnumerable<Type> FindInAssemblyAndReferencedAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(FindInAssemblyAndReferencedAssemblies);

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
