using System;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public partial class MockTable
    {
        /// <summary />
        public static MockTable Empty()
        {
            return new MockTable();
        }

        /// <summary />
        public static MockTable WithColumns(params string[] columns)
        {
            return new MockTable()
            {
                Columns = columns
            };
        }

        /// <summary />
        public static MockTable SingleCell(string columnName, object value)
        {
            return new MockTable()
            {
                Columns = new[] { columnName },
                Rows = new[,] { { value } },
            };
        }

        /// <summary />
        public static MockTable SingleCell(object value)
        {
            return SingleCell(String.Empty, value);
        }

        /// <summary />
        public static MockTable FromCsv(string content, string delimiter, ImportOptions options)
        {
            var table = MockTable.Empty();
            bool isFirstRow = true;
            bool mustRemoveEmptyLines = (options & ImportOptions.RemoveEmptyLines) == ImportOptions.RemoveEmptyLines;
            bool mustTrimLines = (options & ImportOptions.TrimLines) == ImportOptions.TrimLines;

            foreach (string row in content.Split(Environment.NewLine))
            {
                if (mustRemoveEmptyLines && string.IsNullOrEmpty(row))
                {

                }
                else
                {
                    string[] data;

                    if (mustTrimLines)
                        data = row.Trim().Split(delimiter);
                    else
                        data = row.Split(delimiter);

                    if (isFirstRow)
                        table.AddColumns(data);
                    else
                        table.AddRow(data);

                    isFirstRow = false;
                }
            }

            return table;
        }

        /// <summary />
        public static MockTable FromCsv(string content, string delimiter)
        {
            return FromCsv(content, delimiter, ImportOptions.RemoveEmptyLines | ImportOptions.TrimLines);
        }

        /// <summary />
        public static MockTable FromCsv(string content)
        {
            return FromCsv(content, "\t", ImportOptions.RemoveEmptyLines | ImportOptions.TrimLines);
        }
    }

    public enum ImportOptions
    {
        None = 0,
        RemoveEmptyLines = 1,
        TrimLines = 2,
    }
}
