using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model.Parsing
{
    static class TagParser
    {
        public static IEnumerable<Tag> ParseTagsIfPresent(LineReader reader)
        {
            if (!reader.IsTagLine)
            {
                return Enumerable.Empty<Tag>();
            }

            var tags = new List<Tag>();

            while (reader.IsTagLine)
            {
                var tagsOnLine = reader.CurrentLineTrimmed.Split(',');
                foreach (var tag in tagsOnLine)
                {
                    var cleanTag = tag.Trim();
                    if (!cleanTag.StartsWith("@"))
                    {
                        throw new InvalidGherkinSyntaxException(
                            "All tags must start with @.",
                            reader.CurrentLineNumber);
                    }

                    cleanTag = cleanTag
                        .Substring(1)
                        .TrimStart();

                    tags.Add(new Tag(cleanTag));
                }

                reader.ReadNextLine();
            }

            return tags;
        }
    }
}
