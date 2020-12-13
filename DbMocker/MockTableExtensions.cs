using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    public static class MockTableExtensions
    {
        private static readonly ConcurrentDictionary<Type, (string[] columns, MemberInfo[] props)> ColumnMetaStore
               = new ConcurrentDictionary<Type, (string[] columns, MemberInfo[] props)>();

        /// <summary>
        /// Convert object to MockTable
        /// </summary>
        /// <param name="obj">source object</param>
        /// <param name="configureColumnOrder">Function sets column sequence, for only selected members</param>
        /// <returns>MockTable with a single row</returns>
        public static MockTable ToMockTable<T>(this T obj, Action<ColumnSequenceContainer<T>> configureColumnOrder = null)
        {
            var columnMeta = GetColumnMeta(configureColumnOrder);

            return ToTable(new [] { obj }, columnMeta);
        }

        /// <summary>
        /// Convert object to MockTable
        /// </summary>
        /// <typeparam name="T">Use only with ValueTuple</typeparam>
        /// <param name="obj">Source object</param>
        /// <param name="names">Column names in the order listed</param>
        /// <returns>MockTable with a single row</returns>
        public static MockTable ToMockTable<T>(this T obj, params string[] names) where T : struct
        {
            var columnMeta = GetColumnMeta<T>(s => s.TupleRename(names));

            return ToTable(new[] { obj }, columnMeta);
        }

        /// <summary>
        /// Create table header without data
        /// </summary>
        /// <param name="configureColumnOrder">Function sets column sequence, for only selected members</param>
        /// <returns>Empty MockTable</returns>
        public static MockTable GetEmptyMockTable<T>(this T source, Action<ColumnSequenceContainer<T>> configureColumnOrder = null)
        {
            var columnMeta = GetColumnMeta(configureColumnOrder);

            return ToTable(Enumerable.Empty<T>(), columnMeta);
        }

        /// <summary>
        /// Create table header without data
        /// </summary>
        /// <typeparam name="T">Use only with ValueTuple</typeparam>
        /// <param name="names">Column names in the order listed</param>
        /// <returns>Empty MockTable</returns>
        public static MockTable GetEmptyMockTable<T>(this T source, params string[] names) where T : struct
        {
            var columnMeta = GetColumnMeta<T>(s => s.TupleRename(names));

            return ToTable(Enumerable.Empty<T>(), columnMeta);
        }

        /// <summary>
        /// Create table header without data
        /// </summary>
        /// <param name="configureColumnOrder">The function sets column order for selected</param>
        /// <returns>Empty MockTable</returns>
        public static MockTable GetEmptyMockTable<T>(Action<ColumnSequenceContainer<T>> configureColumnOrder = null)
        {
            var columnMeta = GetColumnMeta(configureColumnOrder);

            return ToTable(Enumerable.Empty<T>(), columnMeta);
        }


        /// <summary>
        /// Create table header without data
        /// </summary>
        /// <typeparam name="T">Use only with ValueTuple</typeparam>
        /// <param name="names">Column names in the order listed</param>
        /// <returns>Empty MockTable</returns>
        public static MockTable GetEmptyMockTable<T>(params string[] names) where T : struct
        {
            var columnMeta = GetColumnMeta<T>(s => s.TupleRename(names));

            return ToTable(Enumerable.Empty<T>(), columnMeta);
        }

        /// <summary>
        /// Convert collection to MockTable
        /// </summary>
        /// <param name="collection">source collection</param>
        /// <param name="configureColumnOrder">The function sets column order</param>
        /// <returns>MockTable with data</returns>
        public static MockTable ToMockTableFromCollection<T>(this IEnumerable<T> collection, Action<ColumnSequenceContainer<T>> configureColumnOrder = null)
        {
            var columnMeta = GetColumnMeta(configureColumnOrder);

            return ToTable(collection, columnMeta);
        }

        /// <summary>
        /// Use this method to convert Collection<ValueTuple> to MockTable
        /// </summary>
        /// <typeparam name="T">Use only with ValueTuple</typeparam>
        /// <param name="collection">source collection</param>
        /// <param name="names">Column names in the order listed</param>
        /// <returns>MockTable with data</returns>
        public static MockTable ToMockTableFromCollection<T>(this IEnumerable<T> collection, params string[] names) where T : struct
        {
            var columnMeta = GetColumnMeta<T>(s => s.TupleRename(names));

            return ToTable(collection, columnMeta);
        }

        /// <summary>
        /// Create meta data about object and save to cash
        /// </summary>
        /// <param name="configureColumnSequence">Function sets column sequence, for only selected members</param>
        /// <returns>
        /// columns - the table's column's names,
        /// props - property info
        /// </returns>
        private static (string[] columns, MemberInfo[] props) GetColumnMeta<T>(Action<ColumnSequenceContainer<T>> configureColumnOrder)
        {
            var columnSequence = new ColumnSequenceContainer<T>();
            configureColumnOrder?.Invoke(columnSequence);
            if (columnSequence.HasColumn)
            {
                return columnSequence.GetMeta();
            }

            Type type = typeof(T);

            if (!ColumnMetaStore.TryGetValue(type, out (string[] columns, MemberInfo[] props) columnMeta))
            {
                var props = type.GetSuportedMembers();
                columnMeta = (props.Select(s => s.Name).ToArray(), props);
                ColumnMetaStore.TryAdd(type, columnMeta);
            }

            return columnMeta;
        }

        internal static MemberInfo[] GetSuportedMembers(this Type type)
        {
            return type.GetMembers().Where(w => w.MemberType == MemberTypes.Property || w.MemberType == MemberTypes.Field).ToArray(); 
        }

        private static MockTable ToTable<T>(IEnumerable<T> collection, (string[] columns, MemberInfo[] props) columnMeta) 
        {
            var mockTable = new MockTable();

            mockTable.AddColumns(columnMeta.columns);

            foreach (var item in collection)
            {
                object[] values = new object[columnMeta.props.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = GetValue(columnMeta.props[i], item);
                }

                mockTable.AddRow(values);
            }

            return mockTable;
        }

        private static object GetValue<T>(MemberInfo memberInfo, T item)
        {
            switch (memberInfo)
            {
                case PropertyInfo pi:
                    return pi.GetValue(item);
                case FieldInfo fi: 
                default:
                    return ((FieldInfo)memberInfo).GetValue(item);
            }
        }
    }
}