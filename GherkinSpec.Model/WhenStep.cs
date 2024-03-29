﻿namespace GherkinSpec.Model
{
    public class WhenStep : IStep
    {
        public WhenStep(string title, DataTable tableArgument, string multiLineStringArgument)
        {
            Title = title;
            TableArgument = tableArgument;
            MultiLineStringArgument = multiLineStringArgument;
        }

        public string Title { get; }

        public DataTable TableArgument { get; }

        public string MultiLineStringArgument { get; }

        public string TitleAfterType
            => StepTitleParser.GetTitleAfterType(Title, Resources.WhenPrefixes);

        public IStep CreateAnother(string newText, DataTable tableArgument, string multiLineStringArgument)
            => new WhenStep(newText, tableArgument, multiLineStringArgument);
    }
}
