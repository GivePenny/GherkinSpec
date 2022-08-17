using System;
using System.Linq;

namespace GherkinSpec.Model.Parsing
{
    public static class EventuallyConsistentTagParser
    {
        private const string EventuallyConsistentTagLabelStartUpper = "EVENTUALLYCONSISTENT";
        private const string WithinSettingStartUpper = "WITHIN=";
        private const string RetryIntervalSettingStartUpper = "RETRYINTERVAL=";

        public static bool TryParse(string label, out EventuallyConsistentScenarioConfiguration eventuallyConsistentScenarioConfiguration)
        {
            if (!label
                .StartsWith(
                    EventuallyConsistentTagLabelStartUpper,
                    StringComparison.OrdinalIgnoreCase))
            {
                eventuallyConsistentScenarioConfiguration = null;
                return false;
            }

            var settings = new EventuallyConsistentScenarioConfiguration();

            var labelSettings = label
                .ToUpperInvariant()
                .Replace(EventuallyConsistentTagLabelStartUpper, string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Split(';')
                .Select(s => s.Trim())
                .ToArray();

            if (TimeSpan
                    .TryParse(
                        labelSettings
                        .FirstOrDefault(
                            s => s
                                .StartsWith(
                                    WithinSettingStartUpper,
                                    StringComparison.OrdinalIgnoreCase))
                        ?.Replace(WithinSettingStartUpper, string.Empty),
                        out var withinTimeSpan))
            {
                settings.Within = withinTimeSpan;
            }

            if (TimeSpan
                    .TryParse(
                        labelSettings
                        .FirstOrDefault(
                            s => s
                                .StartsWith(
                                    RetryIntervalSettingStartUpper,
                                    StringComparison.OrdinalIgnoreCase))
                        ?.Replace(RetryIntervalSettingStartUpper, string.Empty),
                        out var retryIntervalTimeSpan))
            {
                settings.RetryInterval = retryIntervalTimeSpan;
            }

            eventuallyConsistentScenarioConfiguration = settings;
            return true;
        }
    }
}
