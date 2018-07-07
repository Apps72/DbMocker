using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockDbTransaction : DbTransaction
    {
        private DbConnection _connection;

        internal MockDbTransaction(DbConnection connection)
        {
            _connection = connection;
        }

        public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;

        protected override DbConnection DbConnection => _connection;

        public override void Commit()
        {

        }

        public override void Rollback()
        {

        }
    }
}
