using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class DataTableRow
    {
        public DataTableRow(IEnumerable<DataTableCell> cells)
        {
            Cells = new ReadOnlyDataTableCellCollection(
                cells.ToList());
        }

        public ReadOnlyDataTableCellCollection Cells { get; }
    }
}
