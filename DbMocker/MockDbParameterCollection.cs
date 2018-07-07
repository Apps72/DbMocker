using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockDbParameterCollection : DbParameterCollection
    {
        private List<MockDbParameter> _parameters = new List<MockDbParameter>();

        public override int Count => _parameters.Count();

        public override object SyncRoot => null;

        public override bool IsSynchronized => true;

        public override bool IsReadOnly => false;

        public override bool IsFixedSize => false;

        public override int Add(object value)
        {
            _parameters.Add(new MockDbParameter(value as DbParameter));
            return _parameters.Count();
        }

        public override void AddRange(Array values)
        {
            _parameters.AddRange(values.Cast<DbParameter>().Select(p => new MockDbParameter(p)));
        }

        public override void Clear()
        {
            _parameters.Clear();
        }

        public override bool Contains(object value)
        {
            var dbParam = value as DbParameter;

            if (dbParam != null)
                return this.Contains(dbParam.ParameterName);
            else
                return false;
        }

        public override bool Contains(string value)
        {
            return _parameters.Any(p => String.Compare(p.ParameterName, value, ignoreCase: true) == 0);
        }

        public override void CopyTo(Array array, int index)
        {
            
        }

        public override IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public override int IndexOf(object value)
        {
            return 0;
        }

        public override int IndexOf(string parameterName)
        {
            return 0;
        }

        public override void Insert(int index, object value)
        {
            _parameters.Add(new MockDbParameter(value as DbParameter));
        }

        public override void Remove(object value)
        {
            
        }

        public override void RemoveAt(int index)
        {
            
        }

        public override void RemoveAt(string parameterName)
        {
            
        }

        protected override DbParameter GetParameter(int index)
        {
            return _parameters.Skip(index)?.FirstOrDefault();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            return _parameters.FirstOrDefault(p => String.Compare(p.ParameterName, parameterName, ignoreCase: true) == 0);
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            var dbParam = this.GetParameter(index);
            if (dbParam != null)
                dbParam = value;
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var dbParam = this.GetParameter(parameterName);
            if (dbParam != null)
                dbParam = value;
        }
    }
}
