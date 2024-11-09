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
                Columns = DbMocker.Columns.WithNames(columns)
            };
        }

        public static MockTable WithColumns(params (string Name, Type Type)[] columns)
        {
            return new MockTable()
            {
                Columns = DbMocker.Columns.WithNames(columns)
            };
        }

        /// <summary />
        public static MockTable SingleCell(string columnName, object value)
        {
            return new MockTable()
            {
                Columns = DbMocker.Columns.WithNames(columnName),
                Rows = new[,] { { value } },
            };
        }

        /// <summary />
        public static MockTable SingleCell(object value)
        {
            return SingleCell(String.Empty, value);
        }
    }
}
