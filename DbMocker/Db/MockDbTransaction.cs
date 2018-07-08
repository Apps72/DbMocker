using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockDbTransaction : DbTransaction
    {
        #region LEGACY METHODS

        private DbConnection _connection;

        /// <summary />
        internal MockDbTransaction(DbConnection connection)
        {
            _connection = connection;
        }

        /// <summary />
        public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

        /// <summary />
        protected override DbConnection DbConnection => _connection;

        /// <summary />
        public override void Commit()
        {

        }

        /// <summary />
        public override void Rollback()
        {

        }

        #endregion
    }
}
