using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker.Data
{
    public class MockDbDataReader : DbDataReader
    {
        private MockTable[] tables;
        private int _currentTableIndex = -1;
        private MockColumn[] _columns;
        private object[,] _rows;
        private int _currentRowIndex = -1;
        private DataTable _schema;
        private bool _isClosed = false;

        internal MockDbDataReader(MockTable[] tables)
        {
            this.tables = tables;
            NextResult();
        }

        #region LEGACY METHODS

        public override object this[int ordinal] => GetValue(ordinal);

        public override object this[string name] => GetValue(GetOrdinal(name));

        public override int Depth => 0;

        public override int FieldCount => _columns?.Length ?? 0;

        public override bool HasRows => _rows?.Length >= 1;

        public override bool IsClosed => _isClosed;

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
            var byteArray = (byte[])GetValue(ordinal);
            Array.Copy(byteArray, dataOffset, buffer, bufferOffset, length);
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
            return _columns[ordinal].Type.Name;
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
            if (ordinal < _columns.Length)
                return _columns[ordinal].Type;
            else
                return GetValue(ordinal).GetType();
        }

        public override float GetFloat(int ordinal)
        {
            return (float)GetValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            object value = GetValue(ordinal);
            if (value.GetType() == typeof(string))
            {
                return new Guid(Convert.ToString(value));
            }
            else
            {
                return (Guid)value;
            }
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
            return _columns[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            for (int i = 0; i < _columns.Length; i++)
            {
                if (String.Compare(_columns[i].Name, name, ignoreCase: true) == 0)
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
            return _rows?[_currentRowIndex, ordinal];
        }

        public override int GetValues(object[] values)
        {
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = _rows?[_currentRowIndex, i];
                }
                return values.Length;
            }

            return 0;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetValue(ordinal) == DBNull.Value ||
                  GetValue(ordinal) == null;
        }

        public override bool NextResult()
        {
            _currentTableIndex++;

            if (_currentTableIndex >= tables.Length)
            {
                _isClosed = true;
                return false;
            }

            _columns = tables[_currentTableIndex].Columns ?? Array.Empty<MockColumn>();
            _rows = tables[_currentTableIndex].Rows ?? new object[,] { };
            _currentRowIndex = -1;
            _schema = Helpers.MockDataReaderSchemaTableBuilder.BuildSchema(_columns, _rows);

            return true;
        }

        public override bool Read()
        {
            _currentRowIndex++;
            return _rows?.GetLength(0) > _currentRowIndex;
        }

        public override DataTable GetSchemaTable()
        {
            return _schema;
        }

        #endregion
    }
}
