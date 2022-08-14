using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GherkinSpec.TestModel;

namespace GherkinSpec.TestModel
{
    public static class StepsClasses
    {
        // Test runners use an isolated appdomain so the cache can be static as it will be discarded after each test run or when the assembly
        // is changed and re-built.
        private static readonly ConcurrentDictionary<Assembly, (Type[], BindingTypesAttribute)> CachedTypesByAssembly =
            new();

        public static IEnumerable<(IEnumerable<Type> Types, BindingTypesAttribute BindingTypes)> FindInAssemblyAndReferencedAssemblies(Assembly assembly)
        {
            return assembly
                .GetReferencedAssemblies()
                .Select(
                    a =>
                        Assembly.Load(a.FullName))
                .Select(FindIn)
                .Append(FindIn(assembly));
        }

        public static IEnumerable<(IEnumerable<Type> Types, BindingTypesAttribute BindingTypes)> FindInAssemblyAndReferencedAssemblies(IEnumerable<Assembly> assemblies)
            => assemblies.SelectMany(FindInAssemblyAndReferencedAssemblies);

        public static IEnumerable<(IEnumerable<Type> Types, BindingTypesAttribute BindingTypes)> FindIn(IEnumerable<Assembly> assemblies)
            => assemblies.Select(FindIn);

        public static (IEnumerable<Type> Types, BindingTypesAttribute BindingTypes) FindIn(Assembly assembly)
        {
            var bindingTypes = (BindingTypesAttribute)assembly
                   .GetCustomAttributes(typeof(BindingTypesAttribute), true)
                   .FirstOrDefault()
               ?? new BindingTypesAttribute();

            return CachedTypesByAssembly.GetOrAdd(
                assembly,
                keyAssembly => (keyAssembly
                    .GetTypes()
                    .Where(
                        type => type.IsClass
                                && type.IsPublic
                                && type.GetCustomAttributes(
                                    bindingTypes.StepsClassAttributeType,
                                    true).Any())
                    .ToArray(),
                        bindingTypes));
        }
    }
}