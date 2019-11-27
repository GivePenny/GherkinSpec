using System;

namespace GherkinSpec.Model
{
    static class StepTitleParser
    {
        public static string GetTitleAfterType(string title, string[] currentStepTitlePrefixes)
        {
            if(Localisation.StartsWithLocalisedValue(title, currentStepTitlePrefixes, out var matchedPrefix))
            {
                return title.Substring(matchedPrefix.Length);
            }

            if (Localisation.StartsWithLocalisedValue(title, Resources.AndPrefixes, out matchedPrefix))
            {
                return title.Substring(matchedPrefix.Length);
            }

            if (Localisation.StartsWithLocalisedValue(title, Resources.ButPrefixes, out matchedPrefix))
            {
                return title.Substring(matchedPrefix.Length);
            }

            throw new InvalidOperationException(
                $"Unexpected format of step title found after successful parsing: {title}");
        }
    }
}
