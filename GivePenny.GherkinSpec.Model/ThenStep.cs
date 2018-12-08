namespace GivePenny.GherkinSpec.Model
{
    public class ThenStep : IStep
    {
        public ThenStep(string title, DataTable tableArgument)
        {
            Title = title;
            TableArgument = tableArgument;
        }

        public string Title { get; }
        public DataTable TableArgument { get; }

        public string TitleAfterType => (Title.StartsWith("Then ") ? Title.Substring(5) : Title.Substring(4)).Trim();

        public IStep CreateAnother(string newText, DataTable tableArgument)
            => new ThenStep(newText, tableArgument);
    }
}
