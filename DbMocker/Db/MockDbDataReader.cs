using System;
using System.Collections;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockDbDataReader : DbDataReader
    {
        private List<string> _columns = new List<string>();
        private List<object[]> _rows = new List<object[]>();
        private int currentRowIndex = 0;

        internal MockDbDataReader(object result)
        {
            object[,] data = result as object[,];

            if (data == null)
                throw new ArgumentException("Returns object must be a bi-dimensional array.");

            var dataConverted = this.ConvertArrayToColsAndRows(data);
            _columns = dataConverted.Columns;
            _rows = dataConverted.Rows;
        }

        /// <summary>
        /// Convert an object[,] to Columns / Rows
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private (List<string> Columns, List<object[]> Rows) ConvertArrayToColsAndRows(object[,] data)
        {
            var cols = new List<string>();
            var rows = new List<object[]>();
            int rowCount = data.GetUpperBound(0);
            int colCount = data.GetUpperBound(1);

            for (int c = 0; c <= colCount; c++)
            {
                cols.Add(Convert.ToString(data[0, c]));
            }

            for (int r = 1; r <= rowCount; r++)
            {
                object[] row = new object[colCount + 1];
                for (int c = 0; c <= colCount; c++)
                {
                    row[c] = data[r, c];
                }
                rows.Add(row);
            }

            return (cols, rows);
        }

        #region LEGACY METHODS

        public override object this[int ordinal] => throw new NotImplementedException();

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => 0;

        public override int FieldCount => _columns.Count;

        public override bool HasRows => _rows.Count > 1;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            currentRowIndex++;
            return _rows.Count > currentRowIndex;
        }

        #endregion
    }
}
