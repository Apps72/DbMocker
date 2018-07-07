using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockDbConnection : DbConnection
    {
        public MockDbConnection()
        {
            this.Mocks = new MockManager(this);
        }

        public MockManager Mocks;

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
        public override ConnectionState State { get; } = ConnectionState.Closed;

        internal DbTransaction Transaction { get; set; }

        /// <summary />
        public override void ChangeDatabase(string databaseName)
        {
        }

        /// <summary />
        public override void Close()
        {
            //this.State = ConnectionState.Closed;
        }

        /// <summary />
        public override void Open()
        {
            //this.State = ConnectionState.Open;
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
