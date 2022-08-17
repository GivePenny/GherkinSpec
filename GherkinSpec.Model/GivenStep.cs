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
            => StepTitleParser.GetTitleAfterType(Title, Resources.GivenPrefixes);        

        public IStep CreateAnother(string newTitle, DataTable tableArgument, string multiLineStringArgument)
            => new GivenStep(newTitle, tableArgument, multiLineStringArgument);
    }
}
