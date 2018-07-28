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
                Columns = new [] { columnName },
                Rows = new[,] { { value } },
            };
        }

        /// <summary />
        public static MockTable SingleCell(object value)
        {
            return SingleCell(String.Empty, value);
        }

        public static MockTable FromCsv(string content, string delimiter)
        {
            var table = MockTable.Empty();
            bool isFirstRow = true;

            foreach (string row in content.Split(Environment.NewLine))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    var data = row.Split(delimiter);
                    if (isFirstRow)
                        table.AddColumns(data);
                    else
                        table.AddRow(data);
                    isFirstRow = false;
                }
            }

            return table;
        }
    }
}
