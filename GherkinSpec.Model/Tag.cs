namespace GherkinSpec.Model
{
    public class Tag
    {
        public Tag(string label)
        {
            Label = label;
        }

        public string Label { get; }

        public override string ToString()
            => Label;
    }
}
