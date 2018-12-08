using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class DataTableRow : IEnumerable<DataTableCell>
    {
        public DataTableRow(IEnumerable<DataTableCell> cells)
        {
            Cells = new ReadOnlyDataTableCellCollection(
                cells.ToList());
        }

        public ReadOnlyDataTableCellCollection Cells { get; }

        public IEnumerator<DataTableCell> GetEnumerator()
            => Cells.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Cells.GetEnumerator();
    }
}
