namespace GivePenny.GherkinCore.Model
{
    public class WhenStep : IStep
    {
        public WhenStep(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public string TitleAfterType => (Title.StartsWith("When ") ? Title.Substring(5) : Title.Substring(4)).Trim();

        public IStep CreateAnother(string newText)
            => new WhenStep(newText);
    }
}
