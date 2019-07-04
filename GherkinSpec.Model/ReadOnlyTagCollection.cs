using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GherkinSpec.Model
{
    public class ReadOnlyTagCollection : ReadOnlyCollection<Tag>
    {
        public ReadOnlyTagCollection(IList<Tag> list)
            : base(list)
        {
        }

        public IEnumerable<string> CategoryNames
        {
            get
            {
                const int lengthOfCategoryKeywordAndOpeningBracket = 9;

                foreach (var tag in this)
                {
                    if (!tag.Label.StartsWith("category(", StringComparison.OrdinalIgnoreCase)
                        || !tag.Label.EndsWith(")", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    yield return tag.Label.Substring(lengthOfCategoryKeywordAndOpeningBracket, tag.Label.Length - lengthOfCategoryKeywordAndOpeningBracket - 1);
                }
            }
        }
    }
}
