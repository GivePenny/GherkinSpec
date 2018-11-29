using System.IO;
using System.Reflection;

namespace GivePenny.GherkinSpec.Model.UnitTests
{
    internal static class Resources
    {
        public static string GetString(string resourceName)
        {
            using (var reader = new StreamReader(GetStream(resourceName)))
            {
                return reader.ReadToEnd();
            }
        }

        private static Stream GetStream(string resourceName)
        {
            resourceName = $"{typeof(Resources).FullName}.{resourceName}";
            return typeof(Resources).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName);
        }
    }
}
