using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockDbConnection : DbConnection
    {
        private ConnectionState _connectionState = ConnectionState.Closed;

        /// <summary />
        public MockDbConnection()
        {
            this.Mocks = new MockConditions(this);
        }

        /// <summary>
        /// Gets the list of database mocks.
        /// </summary>
        public MockConditions Mocks { get; private set; }

        /// <summary>
        /// Gets or sets the default value to validate queries (CommandText).
        /// </summary>
        public bool HasValidSqlServerCommandText
        {
            get
            {
                return this.Mocks.MustValidateSqlServerCommandText;
            }
            set
            {
                this.Mocks.MustValidateSqlServerCommandText = value;
            }
        }

        #region LEGACY METHODS

        /// <summary />
        public override string ConnectionString { get; set; } = "Server=DbMockerServer;Database=DbMockerDatabase";

        /// <summary />
        public override string Database => "DbMockerDatabase";

        /// <summary />
        public override string DataSource => "DbMockerServer";

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
        protected override void Dispose(bool disposing)
        {
            Close();
            base.Dispose(disposing);
        }

        /// <summary />
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (this.Transaction == null)
                this.Transaction = new Data.MockDbTransaction(this);

            return this.Transaction;
        }

        /// <summary />
        protected override DbCommand CreateDbCommand()
        {
            return new Data.MockDbCommand(this);
        }

        public override DataTable GetSchema()
        {
            return this.GetSchema(null, null);
        }

        public override DataTable GetSchema(string collectionName)
        {
            return this.GetSchema(null, null);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            var table = new DataTable("DbMockerSchema");

            table.Columns.AddRange(new[]
            {
                new DataColumn("DataType", typeof(string)),
                new DataColumn("TypeName", typeof(string)),
                new DataColumn("ProviderDbType", typeof(string)),
            });
            
            table.Rows.Add("System.Byte[]", "binary", "1");
            table.Rows.Add("System.Byte", "tinyint", "2");
            table.Rows.Add("System.Boolean", "bit", "3");
            table.Rows.Add("System.Guid", "uniqueidentifier", "4");
            table.Rows.Add("System.DateTime", "datetime", "6");
            table.Rows.Add("System.Decimal", "numeric", "7");
            table.Rows.Add("System.Double", "float", "8");
            table.Rows.Add("System.Int16", "smallint", "10");
            table.Rows.Add("System.Int32", "int", "11");
            table.Rows.Add("System.Int64", "bigint", "12");
            table.Rows.Add("System.Single", "single", "15");
            table.Rows.Add("System.String", "varchar", "16");

            return table;
        }

        #endregion
    }
}
