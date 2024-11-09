using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Apps72.Dev.Data.DbMocker.Helpers
{
    /// <summary />
    internal static class MockDataReaderSchemaTableBuilder
    {        
        private static readonly Dictionary<DbType, DbTypeSchemaDescriptor> _dbTypeDescriptors = InitalizeDbTypeDescriptor();

        /// <summary />
        internal static DataTable BuildSchema(MockColumn[] columns, object[,] rows)
        {
            DataRow row;
            DataTable schemaTable = new DataTable("SchemaTable");

            schemaTable.Locale = CultureInfo.InvariantCulture;
            schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.NumericScale, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.DataType, typeof(Type));
            schemaTable.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.ProviderType, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(Boolean));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.DefaultValue, typeof(object));

            schemaTable.BeginLoadData();

            for (int i = 0; i < columns.Length; i++)
            {
                var column = columns[i];
                row = schemaTable.NewRow();

                bool canGetTypeFromFirstRow = rows.GetLength(0) > 0 && rows.GetLength(1) > i && rows[0, i] != null;
                Type columnType = (column.Type == typeof(object) && canGetTypeFromFirstRow) ? rows[0, i].GetType() : column.Type;
                Type rawColumType = GetFieldType(columnType);
                DbType dbColumType = DbTypeMap.FirstDbType(rawColumType);
                DbTypeSchemaDescriptor dbTypeDescriptor = GetDbTyeDescriptor(dbColumType);

                row[SchemaTableColumn.ColumnName] = column.Name;
                row[SchemaTableColumn.BaseColumnName] = column.Name;
                row[SchemaTableColumn.ColumnOrdinal] = i;
                row[SchemaTableColumn.ColumnSize] = dbTypeDescriptor.ColumnSize;
                row[SchemaTableColumn.NumericPrecision] = dbTypeDescriptor.NumericPrecision;
                row[SchemaTableColumn.NumericScale] = dbTypeDescriptor.NumericScale;
                row[SchemaTableColumn.ProviderType] = dbTypeDescriptor.DbType;
                row[SchemaTableColumn.AllowDBNull] = IsNullable(columnType);

                row[SchemaTableColumn.IsUnique] = false;
                row[SchemaTableOptionalColumn.IsReadOnly] = false;
                row[SchemaTableOptionalColumn.IsRowVersion] = false;
                row[SchemaTableOptionalColumn.IsAutoIncrement] = false;
                row[SchemaTableColumn.DataType] = rawColumType;
                row[SchemaTableOptionalColumn.IsHidden] = false;
                row[SchemaTableColumn.BaseSchemaName] = column.Name;


                row[SchemaTableColumn.IsExpression] = false;
                row[SchemaTableColumn.IsAliased] = false;

                row[SchemaTableColumn.BaseTableName] = DBNull.Value;
                row[SchemaTableOptionalColumn.BaseCatalogName] = DBNull.Value;

                schemaTable.Rows.Add(row);
            }

            schemaTable.AcceptChanges();
            schemaTable.EndLoadData();

            return schemaTable;
        }

        /// <summary />
        private static bool IsNullable(Type type)
        {
            if (!type.IsValueType)
                return true; // ref-type

            if (Nullable.GetUnderlyingType(type) != null)
                return true; // Nullable<T>

            return false; // value-type
        }

        /// <summary />
        private static Type GetFieldType(Type type)
        {
            if (!type.IsValueType)
                return type;

            Type nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
                return nullableType;

            return type;
        }

        /// <summary />
        private static DbTypeSchemaDescriptor GetDbTyeDescriptor(DbType dbType)
        {
            if (_dbTypeDescriptors.ContainsKey(dbType))
                return _dbTypeDescriptors[dbType];

            return _dbTypeDescriptors[DbType.Object];
        }

        /// <summary />
        private static Dictionary<DbType, DbTypeSchemaDescriptor> InitalizeDbTypeDescriptor()
        {
            var dbTypeDescriptors = new Dictionary<DbType, DbTypeSchemaDescriptor>()
            {
                [DbType.AnsiString] = new DbTypeSchemaDescriptor(DbType.AnsiString, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.Binary] = new DbTypeSchemaDescriptor(DbType.Binary, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.Byte] = new DbTypeSchemaDescriptor(DbType.Byte, 1, 3, 0),
                [DbType.Boolean] = new DbTypeSchemaDescriptor(DbType.Boolean, 1, DBNull.Value, DBNull.Value),
                [DbType.Currency] = new DbTypeSchemaDescriptor(DbType.Currency, 8, 19, 4),
                [DbType.Date] = new DbTypeSchemaDescriptor(DbType.Date, 8, DBNull.Value, DBNull.Value),
                [DbType.DateTime] = new DbTypeSchemaDescriptor(DbType.DateTime, 8, DBNull.Value, DBNull.Value),
                [DbType.Decimal] = new DbTypeSchemaDescriptor(DbType.Decimal, 8, 53, DBNull.Value),
                [DbType.Double] = new DbTypeSchemaDescriptor(DbType.Double, 8, 53, DBNull.Value),
                [DbType.Guid] = new DbTypeSchemaDescriptor(DbType.Guid, 16, DBNull.Value, DBNull.Value),
                [DbType.Int16] = new DbTypeSchemaDescriptor(DbType.Int16, 2, 5, 0),
                [DbType.Int32] = new DbTypeSchemaDescriptor(DbType.Int32, 4, 10, 0),
                [DbType.Int64] = new DbTypeSchemaDescriptor(DbType.Int64, 8, 19, 0),
                [DbType.Object] = new DbTypeSchemaDescriptor(DbType.Object, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.SByte] = new DbTypeSchemaDescriptor(DbType.SByte, 1, 3, 0),
                [DbType.Single] = new DbTypeSchemaDescriptor(DbType.Single, 4, 24, DBNull.Value),
                [DbType.String] = new DbTypeSchemaDescriptor(DbType.String, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.Time] = new DbTypeSchemaDescriptor(DbType.Time, 8, DBNull.Value, DBNull.Value),
                [DbType.UInt16] = new DbTypeSchemaDescriptor(DbType.UInt16, 2, 5, 0),
                [DbType.UInt32] = new DbTypeSchemaDescriptor(DbType.UInt32, 4, 10, 0),
                [DbType.UInt64] = new DbTypeSchemaDescriptor(DbType.UInt64, 8, 19, 0),
                [DbType.VarNumeric] = new DbTypeSchemaDescriptor(DbType.VarNumeric, 8, 53, 0),
                [DbType.AnsiStringFixedLength] = new DbTypeSchemaDescriptor(DbType.AnsiStringFixedLength, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.StringFixedLength] = new DbTypeSchemaDescriptor(DbType.StringFixedLength, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.Xml] = new DbTypeSchemaDescriptor(DbType.Xml, int.MaxValue, DBNull.Value, DBNull.Value),
                [DbType.DateTime2] = new DbTypeSchemaDescriptor(DbType.DateTime2, 8, DBNull.Value, DBNull.Value),
                [DbType.DateTimeOffset] = new DbTypeSchemaDescriptor(DbType.DateTimeOffset, 8, DBNull.Value, DBNull.Value)
            };

            return dbTypeDescriptors;
        }
    }   
}
