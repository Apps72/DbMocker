using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public partial class MockTable
    {
        private List<object[]> _rows = new List<object[]>();

        /// <summary />
        public MockTable()
        {
            this.Columns = Array.Empty<string>();
            this.Rows = null;
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
        public object[,] Rows
        {
            get
            {
                return _rows.ToArray().ToTwoDimensionalArray();
            }
            set
            {
                if (value == null)
                    _rows = new List<object[]>();
                else
                    _rows = new List<object[]>(value.ToJaggedArray());
            }
        }

        /// <summary />
        public MockTable AddRow(params object[] values)
        {
            _rows.Add(values);
            return this;
        }

        /// <summary />
        public MockTable AddColumns(params string[] columns)
        {
            this.Columns.Concat(columns).ToArray();
            return this;
        }

        /// <summary />
        internal object GetFirstColRowOrNull()
        {
            if (Rows != null && Rows.GetLength(0) > 0 && Rows.GetLength(1) > 0)
                return Rows[0, 0];
            else
                return null;
        }

    }
}
