using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GherkinSpec.Model
{
    public class ReadOnlyDataTableCellCollection : ReadOnlyCollection<DataTableCell>
    {
        private readonly string[] columnNames;

        public ReadOnlyDataTableCellCollection(IList<DataTableCell> cells, string[] columnNames)
            : base(cells)
        {
            this.columnNames = columnNames;
        }

        public string CommaDelimitedValues
            => string.Join(
                ", ",
                this.Select(
                    cell => cell.Value));

        public DataTableCell this[string columnName]
        {
            get
            {
                var index = Array.FindIndex(columnNames, test => test == columnName);
                return this[index];
            }
        }
    }
}
