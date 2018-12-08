using System.Collections.Generic;

namespace GivePenny.GherkinSpec.Model.Parsing
{
    static class DataTableParser
    {
        public static DataTable ParseDataTable(LineReader reader)
        {
            var rows = new List<DataTableRow>();

            while (reader.IsTableLine)
            {
                rows.Add(ParseDataTableRow(reader));
                reader.ReadNextLine();
            };

            return new DataTable(rows);
        }

        private static DataTableRow ParseDataTableRow(LineReader reader)
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

            return new DataTableRow(cells);
        }
    }
}
