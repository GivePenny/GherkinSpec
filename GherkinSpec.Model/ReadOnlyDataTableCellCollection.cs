using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GherkinSpec.Model
{
    public class ReadOnlyDataTableCellCollection : ReadOnlyCollection<DataTableCell>
    {
        public ReadOnlyDataTableCellCollection(IList<DataTableCell> cells)
            : base(cells)
        {
        }

        public string CommaDelimitedValues
            => string.Join(
                ", ",
                this.Select(
                    cell => cell.Value));
    }
}
