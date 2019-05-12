using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker.Helpers
{
    /// <summary>
    /// DbType Mapping.
    /// see https://github.com/Apps72/Dev.Data
    /// </summary>
    internal static class DbTypeMap
    {
        private static readonly List<Type2DbType> _dbTypeList = FillDbTypeList();
        
        /// <summary>
        /// Returns the first DbType for this CLR Type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType FirstDbType(Type type)
        {
            if (type == null) return DbType.Object;
            return _dbTypeList.First(i => i.Type == type).DbType;
        }
  
        static List<Type2DbType> FillDbTypeList()
        {
            var list = new List<Type2DbType>();

            list.Add(new Type2DbType(typeof(Int16), DbType.Int16));
            list.Add(new Type2DbType(typeof(Int32), DbType.Int32));
            list.Add(new Type2DbType(typeof(Int64), DbType.Int64));
            list.Add(new Type2DbType(typeof(UInt16), DbType.UInt16));
            list.Add(new Type2DbType(typeof(UInt32), DbType.UInt32));
            list.Add(new Type2DbType(typeof(UInt64), DbType.UInt64));
            list.Add(new Type2DbType(typeof(Boolean), DbType.Boolean));
            list.Add(new Type2DbType(typeof(Byte), DbType.Byte));
            list.Add(new Type2DbType(typeof(SByte), DbType.SByte));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Decimal));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Single));
            list.Add(new Type2DbType(typeof(Double), DbType.Double));
            list.Add(new Type2DbType(typeof(Decimal), DbType.Currency));
            list.Add(new Type2DbType(typeof(Double), DbType.VarNumeric));
            list.Add(new Type2DbType(typeof(Single), DbType.Single));

            list.Add(new Type2DbType(typeof(Char), DbType.String));
            list.Add(new Type2DbType(typeof(String), DbType.String));
            list.Add(new Type2DbType(typeof(String), DbType.AnsiString));
            list.Add(new Type2DbType(typeof(String), DbType.AnsiStringFixedLength));
            list.Add(new Type2DbType(typeof(String), DbType.Xml));

            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime));
            list.Add(new Type2DbType(typeof(DateTime), DbType.Date));
            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime2));
            list.Add(new Type2DbType(typeof(DateTime), DbType.DateTime));
            list.Add(new Type2DbType(typeof(DateTimeOffset), DbType.DateTimeOffset));
            list.Add(new Type2DbType(typeof(DateTime), DbType.Time));
            list.Add(new Type2DbType(typeof(TimeSpan), DbType.Time));

            list.Add(new Type2DbType(typeof(Guid), DbType.Guid));
            list.Add(new Type2DbType(typeof(Object), DbType.Object));
            list.Add(new Type2DbType(typeof(Byte[]), DbType.Binary));
            list.Add(new Type2DbType(typeof(Object), DbType.Object));

            return list;
        }
    }

    /// <summary />
    internal class Type2DbType
    {
        /// <summary />
        public Type2DbType(Type type, DbType dbType)
        {
            Type = type;
            DbType = dbType;
        }

        /// <summary />
        public Type Type { get; }
        /// <summary />
        public DbType DbType { get; }
    }
}
