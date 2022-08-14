using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GherkinSpec.TestModel
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllStepsClassesAsScoped(this IServiceCollection services)
            => AddAllStepsClassesAsScoped(services, Assembly.GetCallingAssembly());

        public static IServiceCollection AddAllStepsClassesAsScoped(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in StepsClasses
                .FindIn(assembly)
                .Types
                .Where(IsNotStaticOrAbstract))
            {
                services.AddScoped(type);
            }

            return services;
        }

        // Static classes are both abstract and sealed.  We want neither abstract nor static.
        // https://stackoverflow.com/questions/2639418/use-reflection-to-get-a-list-of-static-classes
        private static bool IsNotStaticOrAbstract(Type type)
            => !type.IsAbstract;
    }
}
