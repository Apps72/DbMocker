using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    public static class MockTableExtensions
    {
        private static readonly ConcurrentDictionary<Type, (string[], PropertyInfo[])> ColumnMetaStore
               = new ConcurrentDictionary<Type, (string[], PropertyInfo[])>();

        /// <summary>
        /// Convert object to MockTable
        /// </summary>
        /// <param name="obj">source object</param>
        /// <returns>MockTable with a single row</returns>
        public static MockTable ToMockTable<T>(this T obj)
        {
            var columnMeta = GetColumnMeta<T>();

            var mocktable = new MockTable();
            mocktable.AddColumns(columnMeta.columns);
            object[] values = new object[columnMeta.props.Length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = columnMeta.props[i].GetValue(obj);
            }

            mocktable.AddRow(values);
            return mocktable;
        }

        /// <summary>
        /// Create meta data about object and save to cash
        /// </summary>
        /// <returns>
        /// columns - the table's column's names,
        /// props - property info
        /// </returns>
        private static (string[] columns, PropertyInfo[] props) GetColumnMeta<T>()
        {
            var type = typeof(T);
            (string[] columns, PropertyInfo[] props) columnMeta;

            if (!ColumnMetaStore.TryGetValue(type, out columnMeta))
            {
                var props = type.GetProperties();
                columnMeta = (props.Select(s => s.Name).ToArray(), props);
                ColumnMetaStore.TryAdd(type, columnMeta);
            }

            return columnMeta;
        }

        /// <summary>
        /// Create table header without data
        /// </summary>
        /// <returns>Empty MockTable</returns>
        public static MockTable GetEmptyMockTable<T>()
        {
            var columnMeta = GetColumnMeta<T>();
            var mocktable = new MockTable();
            mocktable.AddColumns(columnMeta.columns);
            return mocktable;
        }

        /// <summary>
        /// Convert collection to MockTable
        /// </summary>
        /// <param name="collection">source collection</param>
        /// <returns>MockTable with data</returns>
        public static MockTable ToMockTableFromCollection<T>(this IEnumerable<T> collection)
        {
            (string[] columns, PropertyInfo[] props) columnMeta = GetColumnMeta<T>();

            var mockTable = new MockTable();

            mockTable.AddColumns(columnMeta.columns);

            foreach (var item in collection)
            {
                object[] values = new object[columnMeta.props.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = columnMeta.props[i].GetValue(item);
                }

                mockTable.AddRow(values);
            }

            return mockTable;
        }
    }
}