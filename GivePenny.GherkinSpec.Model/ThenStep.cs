namespace GivePenny.GherkinSpec.Model
{
    public class ThenStep : IStep
    {
        public ThenStep(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public string TitleAfterType => (Title.StartsWith("Then ") ? Title.Substring(5) : Title.Substring(4)).Trim();

        public IStep CreateAnother(string newText)
            => new ThenStep(newText);
    }
}
