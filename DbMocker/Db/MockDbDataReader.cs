using System;
using System.Collections;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockDbDataReader : DbDataReader
    {
        private string[] _columns;
        private object[,] _rows;
        private int _currentRowIndex = -1;

        internal MockDbDataReader(MockTable table)
        {
            _columns = table.Columns ?? Array.Empty<string>();
            _rows = table.Rows ?? new object[,] { };
        }

        #region LEGACY METHODS

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth => 0;

        public override int FieldCount => _columns.Length;

        public override bool HasRows => _rows.Length > 1;

        public override bool IsClosed => false;

        public override int RecordsAffected => 0;

        public override bool GetBoolean(int ordinal)
        {
            return (bool)GetValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return (byte)GetValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return length;
        }

        public override char GetChar(int ordinal)
        {
            return (char)GetValue(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return (long)GetValue(ordinal);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _columns[ordinal].GetType().Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)GetValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return (Decimal)GetValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return (double)GetValue(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            return GetValue(ordinal).GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return (Guid)GetValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return (short)GetValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return (int)GetValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return (long)GetValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return _columns[ordinal];
        }

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < _columns.Length; i++)
            {
                if (_columns[i] == name)
                    return i;
            }
            return -1;
        }

        public override string GetString(int ordinal)
        {
            return (string)GetValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return _rows[_currentRowIndex, ordinal];
        }

        public override int GetValues(object[] values)
        {
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = _rows[_currentRowIndex, i];
                }
                return values.Length;
            }

            return 0;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetValue(ordinal) == null;
        }

        public override bool NextResult()
        {
            return false;       // TODO when using datasets
        }

        public override bool Read()
        {
            _currentRowIndex++;
            return _rows.GetLength(0) > _currentRowIndex;
        }

        #endregion
    }
}
