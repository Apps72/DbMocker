using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockTable
    {
        /// <summary />
        public MockTable()
        {
        }

        /// <summary />
        public MockTable(string[] columns, object[,] rows)
        {
            this.Columns = columns;
            this.Rows = rows;
        }

        /// <summary />
        public string[] Columns { get; set; }

        /// <summary />
        public object[,] Rows { get; set; }

        /// <summary />
        public object GetFirstColRowOrNull()
        {
            if (Rows.GetLength(0) > 0 && Rows.GetLength(1) > 0)
                return Rows[0, 0];
            else
                return null;
        }
    }
}
