using System;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker.Data
{
    /// <summary />
    public class MockDbCommand : DbCommand
    {
        private MockDbConnection _connection;

        /// <summary />
        internal MockDbCommand(MockDbConnection connection)
        {
            _connection = connection;
            this.DbConnection = _connection;
            this.Transaction = _connection.Transaction;
            this.DbParameterCollection = new MockDbParameterCollection();
        }

        /// <summary />
        public override int ExecuteNonQuery()
        {
            int? returns = this.ExecuteScalar() as int?;
            
            if (returns.HasValue)
                return returns.Value;
            else
                return 0;
        }

        /// <summary />
        public override object ExecuteScalar()
        {
            return _connection.Mocks
                              .GetFirstMockTableFound(new MockCommand(this))
                              .GetFirstColRowOrNull();
        }

        /// <summary />
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            var result = _connection.Mocks
                                    .GetFirstMockTableFound(new MockCommand(this));

            return new MockDbDataReader(result);
        }

        #region LEGACY METHODS

        /// <summary />
        public override string CommandText { get; set; }

        /// <summary />
        public override int CommandTimeout { get; set; } = 60;

        /// <summary />
        public override CommandType CommandType { get; set; } = CommandType.Text;

        /// <summary />
        public override bool DesignTimeVisible { get; set; } = false;

        /// <summary />
        public override UpdateRowSource UpdatedRowSource { get; set; } = UpdateRowSource.None;

        /// <summary />
        protected override DbConnection DbConnection { get; set; }

        /// <summary />
        protected override DbParameterCollection DbParameterCollection { get; }

        /// <summary />
        protected override DbTransaction DbTransaction { get; set; }

        /// <summary />
        public override void Cancel()
        {
        }

        /// <summary />
        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new MockDbParameter(this);
        }

        #endregion
    }
}
