namespace GivePenny.GherkinSpec.Model
{
    public interface IStep
    {
        IStep CreateAnother(string newTitle, DataTable tableArgument);
        string Title { get; }
        string TitleAfterType { get; }
        DataTable TableArgument { get; }
    }
}
