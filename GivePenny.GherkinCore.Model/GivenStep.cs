namespace GivePenny.GherkinCore.Model
{
    public class GivenStep : IStep
    {
        public GivenStep(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public string TitleAfterType => (Title.StartsWith("Given ") ? Title.Substring(6) : Title.Substring(4)).Trim();

        public IStep CreateAnother(string newTitle)
            => new GivenStep(newTitle);
    }
}
