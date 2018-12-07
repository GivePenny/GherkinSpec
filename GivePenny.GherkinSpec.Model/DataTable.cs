using System;
using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class DataTable
    {
        public DataTable(IEnumerable<DataTableRow> rows)
        {
            Rows = rows.ToList().AsReadOnly();
        }

        public static DataTable Empty { get; } = new DataTable(
            Enumerable.Empty<DataTableRow>());

        public IEnumerable<string> ColumnNames
            => Rows
                .First()
                .Cells
                .Select(cell => cell.Value);

        public IReadOnlyCollection<DataTableRow> Rows { get; }

        public string ReplacePlaceholdersWithValues(string text, DataTableRow row)
        {
            var cellIndex = 0;
            foreach (var columnName in ColumnNames)
            {
                text = text.Replace("<" + columnName + ">", row.Cells[cellIndex].Value);
                cellIndex++;
            }

            return text;
        }
    }
}
