using System.Resources;

namespace GherkinSpec.Model
{
    static class Resources
    {
        private static readonly ResourceManager resourceManager = new ResourceManager(
            typeof(Resources).FullName + ".LocalisedContent",
            typeof(Resources).Assembly);

        public static string[] AndPrefixes => resourceManager.GetString(nameof(AndPrefixes)).Split(';');

        public static string BackgroundKeyword => resourceManager.GetString(nameof(BackgroundKeyword));

        public static string[] ButPrefixes => resourceManager.GetString(nameof(ButPrefixes)).Split(';');

        public static string ExamplesKeyword => resourceManager.GetString(nameof(ExamplesKeyword));

        public static string FeatureKeyword => resourceManager.GetString(nameof(FeatureKeyword));

        public static string[] GivenPrefixes => resourceManager.GetString(nameof(GivenPrefixes)).Split(';');

        public static string[] IgnoreTagKeywords => resourceManager.GetString(nameof(IgnoreTagKeywords)).Split(';');

        public static string RuleKeyword => resourceManager.GetString(nameof(RuleKeyword));

        public static string[] ScenarioPrefixes => resourceManager.GetString(nameof(ScenarioPrefixes)).Split(';');

        public static string ScenarioOutlineKeyword => resourceManager.GetString(nameof(ScenarioOutlineKeyword));

        public static string[] ThenPrefixes => resourceManager.GetString(nameof(ThenPrefixes)).Split(';');

        public static string[] WhenPrefixes => resourceManager.GetString(nameof(WhenPrefixes)).Split(';');
    }
}
