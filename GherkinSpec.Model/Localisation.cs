using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GherkinSpec.Model
{
    public static class Localisation
    {
        private static readonly Regex CultureTagRegex = new(
            @"(culture|kultur|cultura)\((?<code>[\w\-]{2,5})\)",
            RegexOptions.ExplicitCapture);

        public static void SetUiCultureFromTag(IEnumerable<Tag> featureTags)
        {
            var cultureTag = featureTags.FirstOrDefault(tag => CultureTagRegex.IsMatch(tag.Label));

            if (cultureTag == null)
            {
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                return;
            }

            var cultureCode = CultureTagRegex.Match(cultureTag.Label).Groups[1].Captures[0].Value;

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureCode);
        }

        public static bool IsLocalisedValue(string value, string[] localisedPossibleValues)
        {
            foreach (var possibleValue in localisedPossibleValues)
            {
                if (string.Equals(value, possibleValue, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool StartsWithLocalisedValue(string value, string[] localisedPossibleValues, out string matchedPrefix)
        {
            if (value != null)
            {
                foreach (var possibleValue in localisedPossibleValues)
                {
                    if (value.StartsWith(possibleValue, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedPrefix = possibleValue;
                        return true;
                    }
                }
            }

            matchedPrefix = null;
            return false;
        }
    }
}
