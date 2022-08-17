using System.Collections.Generic;
using System.Linq;

namespace GherkinSpec.Model.Parsing
{
    internal static class DataTableParser
    {
        public static DataTable ParseDataTable(LineReader reader)
        {
            var rows = new List<DataTableRow>();
            string[] columnNames = null;

            while (reader.IsTableLine)
            {
                rows.Add(ParseDataTableRow(reader, ref columnNames));
                reader.ReadNextLine();
            }

            return new DataTable(rows);
        }

        private static DataTableRow ParseDataTableRow(LineReader reader, ref string[] columnNames)
        {
            var column = reader
                .CurrentLineTrimmed
                .RemoveFirstAndLastPipes(
                    reader.CurrentLineNumber)
                .Split('|');

            var cells = new List<DataTableCell>();

            foreach (var value in column)
            {
                cells.Add(
                    new DataTableCell(
                        value.Trim()));
            }

            if (columnNames == null)
            {
                columnNames = cells
                    .Select(cell => cell.Value)
                    .ToArray();
            }

            return new DataTableRow(cells, columnNames);
        }
    }
}
