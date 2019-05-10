using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model
{
    public class DataTableRow : IEnumerable<DataTableCell>
    {
        public DataTableRow(IEnumerable<DataTableCell> cells, string[] columnNames)
        {
            Cells = new ReadOnlyDataTableCellCollection(
                cells.ToList(),
                columnNames);
        }

        public ReadOnlyDataTableCellCollection Cells { get; }

        public IEnumerator<DataTableCell> GetEnumerator()
            => Cells.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Cells.GetEnumerator();

        public DataTableCell this[int index]
            => Cells[index];

        public DataTableCell this[string columnName]
            => Cells[columnName];
    }
}
