using System.Resources;

namespace GherkinSpec.Model
{
    internal static class Resources
    {
        private static readonly ResourceManager ResourceManager = new(
            typeof(Resources).FullName + ".LocalisedContent",
            typeof(Resources).Assembly);

        public static string[] AndPrefixes => ResourceManager.GetString(nameof(AndPrefixes))!.Split(';');

        public static string[] BackgroundPrefixes => ResourceManager.GetString(nameof(BackgroundPrefixes))!.Split(';');

        public static string[] ButPrefixes => ResourceManager.GetString(nameof(ButPrefixes))!.Split(';');

        public static string ExamplesKeyword => ResourceManager.GetString(nameof(ExamplesKeyword));

        public static string[] FeaturePrefixes => ResourceManager.GetString(nameof(FeaturePrefixes))!.Split(';');

        public static string[] GivenPrefixes => ResourceManager.GetString(nameof(GivenPrefixes))!.Split(';');

        public static string[] IgnoreTagKeywords => ResourceManager.GetString(nameof(IgnoreTagKeywords))!.Split(';');

        public static string[] RulePrefixes => ResourceManager.GetString(nameof(RulePrefixes))!.Split(';');

        public static string[] ScenarioPrefixes => ResourceManager.GetString(nameof(ScenarioPrefixes))!.Split(';');

        public static string[] ScenarioOutlinePrefixes => ResourceManager.GetString(nameof(ScenarioOutlinePrefixes))!.Split(';');

        public static string[] ThenPrefixes => ResourceManager.GetString(nameof(ThenPrefixes))!.Split(';');

        public static string[] WhenPrefixes => ResourceManager.GetString(nameof(WhenPrefixes))!.Split(';');
    }
}
