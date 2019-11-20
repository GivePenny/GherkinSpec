using System.Resources;

namespace GherkinSpec.TestAdapter
{
    static class Resources
    {
        private static readonly ResourceManager resourceManager = new ResourceManager(typeof(Resources));

        public static string FeatureKeyword => resourceManager.GetString("FeatureKeyword");
    }
}
