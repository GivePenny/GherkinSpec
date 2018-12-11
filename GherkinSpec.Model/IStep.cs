namespace GherkinSpec.Model
{
    public interface IStep
    {
        IStep CreateAnother(string newTitle, DataTable tableArgument, string multiLineStringArgument);
        string Title { get; }
        string TitleAfterType { get; }
        DataTable TableArgument { get; }
        string MultiLineStringArgument { get; }
    }
}
