using System;

namespace GherkinSpec.Model
{
    public class GivenStep : IStep
    {
        public GivenStep(string title, DataTable tableArgument, string multiLineStringArgument)
        {
            Title = title;
            TableArgument = tableArgument;
            MultiLineStringArgument = multiLineStringArgument;
        }

        public string Title { get; }

        public DataTable TableArgument { get; }

        public string MultiLineStringArgument { get; }

        public string TitleAfterType
        {
            get
            {
                if (Title.StartsWith(Resources.GivenKeyword))
                {
                    return Title.Substring(Resources.GivenKeyword.Length + 1);
                }

                if (Title.StartsWith(Resources.AndKeyword))
                {
                    return Title.Substring(Resources.AndKeyword.Length + 1);
                }

                if (Title.StartsWith(Resources.ButKeyword))
                {
                    return Title.Substring(Resources.ButKeyword.Length + 1);
                }

                throw new InvalidOperationException(
                    $"Unexpected format of step title found after successful parsing: {Title}");
            }
        }        

        public IStep CreateAnother(string newTitle, DataTable tableArgument, string multiLineStringArgument)
            => new GivenStep(newTitle, tableArgument, multiLineStringArgument);
    }
}
