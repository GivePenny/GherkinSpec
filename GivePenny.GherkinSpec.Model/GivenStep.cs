namespace GivePenny.GherkinSpec.Model
{
    public class GivenStep : IStep
    {
        public GivenStep(string title, DataTable tableArgument)
        {
            Title = title;
            TableArgument = tableArgument;
        }

        public string Title { get; }
        public DataTable TableArgument { get; }

        public string TitleAfterType => (Title.StartsWith("Given ") ? Title.Substring(6) : Title.Substring(4)).Trim();

        public IStep CreateAnother(string newTitle, DataTable tableArgument)
            => new GivenStep(newTitle, tableArgument);
    }
}
