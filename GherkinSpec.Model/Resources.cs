using System.Resources;

namespace GherkinSpec.Model
{
    static class Resources
    {
        private static readonly ResourceManager resourceManager = new ResourceManager(
            typeof(Resources).FullName + ".LocalisedContent",
            typeof(Resources).Assembly);

        public static string[] AndPrefixes => resourceManager.GetString(nameof(AndPrefixes)).Split(';');

        public static string[] BackgroundPrefixes => resourceManager.GetString(nameof(BackgroundPrefixes)).Split(';');

        public static string[] ButPrefixes => resourceManager.GetString(nameof(ButPrefixes)).Split(';');

        public static string ExamplesKeyword => resourceManager.GetString(nameof(ExamplesKeyword));

        public static string[] FeaturePrefixes => resourceManager.GetString(nameof(FeaturePrefixes)).Split(';');

        public static string[] GivenPrefixes => resourceManager.GetString(nameof(GivenPrefixes)).Split(';');

        public static string[] IgnoreTagKeywords => resourceManager.GetString(nameof(IgnoreTagKeywords)).Split(';');

        public static string[] RulePrefixes => resourceManager.GetString(nameof(RulePrefixes)).Split(';');

        public static string[] ScenarioPrefixes => resourceManager.GetString(nameof(ScenarioPrefixes)).Split(';');

        public static string[] ScenarioOutlinePrefixes => resourceManager.GetString(nameof(ScenarioOutlinePrefixes)).Split(';');

        public static string[] ThenPrefixes => resourceManager.GetString(nameof(ThenPrefixes)).Split(';');

        public static string[] WhenPrefixes => resourceManager.GetString(nameof(WhenPrefixes)).Split(';');
    }
}
