using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockDbConnection : DbConnection
    {
        private ConnectionState _connectionState = ConnectionState.Closed;

        public MockDbConnection()
        {
            this.Mocks = new MockConditions(this);
        }

        public MockConditions Mocks;

        #region LEGACY METHODS

        /// <summary />
        public override string ConnectionString { get; set; }

        /// <summary />
        public override string Database => "Mock Database";

        /// <summary />
        public override string DataSource => "Mock Data Source";

        /// <summary />
        public override string ServerVersion => "1.0";

        /// <summary />
        public override ConnectionState State { get { return _connectionState; } }

        internal DbTransaction Transaction { get; set; }

        /// <summary />
        public override void ChangeDatabase(string databaseName)
        {
        }

        /// <summary />
        public override void Close()
        {
            _connectionState = ConnectionState.Closed;
        }

        /// <summary />
        public override void Open()
        {
            _connectionState = ConnectionState.Open;
        }

        /// <summary />
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (this.Transaction == null)
                this.Transaction = new MockDbTransaction(this);

            return this.Transaction;
        }

        /// <summary />
        protected override DbCommand CreateDbCommand()
        {
            return new MockDbCommand(this);
        }

        #endregion
    }
}
