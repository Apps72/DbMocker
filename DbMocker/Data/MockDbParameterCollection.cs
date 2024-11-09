using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker.Data
{
    /// <summary />
    public class MockDbParameterCollection : DbParameterCollection
    {
        #region LEGACY METHODS

        private List<MockDbParameter> _parameters = new List<MockDbParameter>();

        /// <summary />
        public override int Count => _parameters.Count();

        /// <summary />
        public override object SyncRoot => null;

        /// <summary />
        public override bool IsSynchronized => true;

        /// <summary />
        public override bool IsReadOnly => false;

        /// <summary />
        public override bool IsFixedSize => false;

        /// <summary />
        public override int Add(object value)
        {
            _parameters.Add(new MockDbParameter(value as DbParameter));
            return _parameters.Count();
        }

        /// <summary />
        public override void AddRange(Array values)
        {
            _parameters.AddRange(values.Cast<DbParameter>().Select(p => new MockDbParameter(p)));
        }

        /// <summary />
        public override void Clear()
        {
            _parameters.Clear();
        }

        /// <summary />
        public override bool Contains(object value)
        {
            var dbParam = value as DbParameter;

            if (dbParam != null)
                return this.Contains(dbParam.ParameterName);
            else
                return false;
        }

        /// <summary />
        public override bool Contains(string value)
        {
            return _parameters.Any(p => String.Compare(p.ParameterName, value, ignoreCase: true) == 0);
        }

        /// <summary />
        public override void CopyTo(Array array, int index)
        {
            
        }

        /// <summary />
        public override IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        /// <summary />
        public override int IndexOf(object value)
        {
            return 0;
        }

        /// <summary />
        public override int IndexOf(string parameterName)
        {
            return 0;
        }

        /// <summary />
        public override void Insert(int index, object value)
        {
            _parameters.Add(new MockDbParameter(value as DbParameter));
        }

        /// <summary />
        public override void Remove(object value)
        {
            
        }

        /// <summary />
        public override void RemoveAt(int index)
        {
            
        }

        /// <summary />
        public override void RemoveAt(string parameterName)
        {
            
        }

        /// <summary />
        protected override DbParameter GetParameter(int index)
        {
            return _parameters.Skip(index)?.FirstOrDefault();
        }

        /// <summary />
        protected override DbParameter GetParameter(string parameterName)
        {
            return _parameters.FirstOrDefault(p => String.Compare(p.ParameterName, parameterName, ignoreCase: true) == 0);
        }

        /// <summary />
        protected override void SetParameter(int index, DbParameter value)
        {
            var dbParam = this.GetParameter(index);
            if (dbParam != null)
                dbParam = value;
        }

        /// <summary />
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var dbParam = this.GetParameter(parameterName);
            if (dbParam != null)
                dbParam = value;
        }

        #endregion
    }
}
