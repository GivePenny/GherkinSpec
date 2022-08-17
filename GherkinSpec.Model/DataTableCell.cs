namespace GherkinSpec.Model
{
    public class DataTableCell
    {
        public DataTableCell(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
            => Value;
    }
}
