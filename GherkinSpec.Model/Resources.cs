using System.Resources;

namespace GherkinSpec.Model
{
    static class Resources
    {
        private static readonly ResourceManager resourceManager = new ResourceManager(
            typeof(Resources).FullName + ".LocalisedContent",
            typeof(Resources).Assembly);

        public static string AndKeyword => resourceManager.GetString(nameof(AndKeyword));

        public static string BackgroundKeyword => resourceManager.GetString(nameof(BackgroundKeyword));

        public static string ButKeyword => resourceManager.GetString(nameof(ButKeyword));

        public static string ExampleKeyword => resourceManager.GetString(nameof(ExampleKeyword));

        public static string ExamplesKeyword => resourceManager.GetString(nameof(ExamplesKeyword));

        public static string FeatureKeyword => resourceManager.GetString(nameof(FeatureKeyword));

        public static string GivenKeyword => resourceManager.GetString(nameof(GivenKeyword));

        public static string RuleKeyword => resourceManager.GetString(nameof(RuleKeyword));

        public static string ScenarioKeyword => resourceManager.GetString(nameof(ScenarioKeyword));

        public static string ScenarioOutlineKeyword => resourceManager.GetString(nameof(ScenarioOutlineKeyword));

        public static string ThenKeyword => resourceManager.GetString(nameof(ThenKeyword));

        public static string WhenKeyword => resourceManager.GetString(nameof(WhenKeyword));
    }
}
