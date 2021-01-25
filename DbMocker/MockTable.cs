using Apps72.Dev.Data.DbMocker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public partial class MockTable
    {
        public static readonly char[] SPLIT_NEWLINE = { '\r', '\n' };
        private List<object[]> _rows = new List<object[]>();

        /// <summary />
        public MockTable()
        {
            this.Columns = Array.Empty<MockColumn>();
            this.Rows = null;
        }

        /// <summary />
        public MockTable(string[] columns, object[,] rows)
        {
            this.Columns = columns.Select(name => new MockColumn(name)).ToArray();
            this.Rows = rows;
        }

        /// <summary />
        public MockColumn[] Columns { get; set; }

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
            this.Columns = this.Columns.Concat(DbMocker.Columns.WithNames(columns)).ToArray();
            return this;
        }

        /// <summary />
        public MockTable AddColumns(params (string Name, Type Type)[] columns)
        {
            this.Columns = this.Columns.Concat(DbMocker.Columns.WithNames(columns)).ToArray();
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
