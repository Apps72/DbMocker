using System;
using System.Data;

namespace Apps72.Dev.Data.DbMocker.Helpers
{
    /// <summary />
    internal class DbTypeSchemaDescriptor
    {
        /// <summary />
        internal DbTypeSchemaDescriptor(DbType dbType, int columnSize, object numericPrecision, object numericScale)
        {
            DbType = dbType;
            ColumnSize = columnSize;
            NumericPrecision = numericPrecision;
            NumericScale = numericScale;
        }

        /// <summary />
        public DbType DbType { get; }

        /// <summary />
        public int ColumnSize { get; }

        /// <summary />
        public object NumericPrecision { get; }

        /// <summary />
        public object NumericScale { get; }
    }
}
