using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GivePenny.GherkinSpec.Model
{
    public class DataTable : IEnumerable<DataTableRow>
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

        public IEnumerator<DataTableRow> GetEnumerator()
            => Rows.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Rows.GetEnumerator();

        public bool IsEmpty
            => Rows.Count == 0;

        public string ReplacePlaceholdersWithValues(string text, DataTableRow rowProvidingValues)
        {
            var cellIndex = 0;
            foreach (var columnName in ColumnNames)
            {
                text = text.Replace("<" + columnName + ">", rowProvidingValues.Cells[cellIndex].Value);
                cellIndex++;
            }

            return text;
        }

        public DataTable ReplacePlaceholdersWithValues(DataTable tableToCloneThenUpdate, DataTableRow rowProvidingValues)
        {
            var targetRows = new List<DataTableRow>();
            foreach (var sourceRow in tableToCloneThenUpdate.Rows)
            {
                var targetCells = new List<DataTableCell>();
                foreach (var sourceCell in sourceRow.Cells)
                {
                    var targetCell = new DataTableCell(
                        ReplacePlaceholdersWithValues(
                            sourceCell.Value,
                            rowProvidingValues));
                    targetCells.Add(targetCell);
                }
                targetRows.Add(new DataTableRow(targetCells));
            }
            return new DataTable(targetRows);
        }
    }
}
