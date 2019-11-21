using System;

namespace GherkinSpec.Model
{
    public class ThenStep : IStep
    {
        public ThenStep(string title, DataTable tableArgument, string multiLineStringArgument)
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
                if (Title.StartsWith(Resources.ThenKeyword))
                {
                    return Title.Substring(Resources.ThenKeyword.Length + 1);
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

        public IStep CreateAnother(string newText, DataTable tableArgument, string multiLineStringArgument)
            => new ThenStep(newText, tableArgument, multiLineStringArgument);
    }
}
