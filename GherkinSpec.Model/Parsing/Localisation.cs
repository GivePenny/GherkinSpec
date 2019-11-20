using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace GherkinSpec.Model.Parsing
{
    public static class Localisation
    {
        private static readonly Regex cultureTagRegex = new Regex(
            @"(culture|kultur)\((?<code>[\w\-]{2,5})\)",
            RegexOptions.ExplicitCapture);

        public static void SetUICultureFromTag(IEnumerable<Tag> featureTags)
        {
            var cultureTag = featureTags.FirstOrDefault(tag => cultureTagRegex.IsMatch(tag.Label));

            if (cultureTag == null)
            {
                CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
                return;
            }

            var cultureCode = cultureTagRegex.Match(cultureTag.Label).Groups[1].Captures[0].Value;

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureCode);
        }
    }
}
