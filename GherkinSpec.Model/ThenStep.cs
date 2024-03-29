﻿namespace GherkinSpec.Model
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
            => StepTitleParser.GetTitleAfterType(Title, Resources.ThenPrefixes);

        public IStep CreateAnother(string newText, DataTable tableArgument, string multiLineStringArgument)
            => new ThenStep(newText, tableArgument, multiLineStringArgument);
    }
}
