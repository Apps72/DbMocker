using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary>
    /// Container for configuring columns sequence
    /// </summary>
    /// <typeparam name="T">Some type</typeparam>
    public class ColumnSequenceContainer<T>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ColumnSequenceContainer()
        {
        }

        /// <summary>
        /// Columns list
        /// </summary>
        protected readonly Queue<(string columnName, MemberInfo member)> Queue = new Queue<(string columnName, MemberInfo member)>();

        /// <summary>
        /// Get table meta
        /// </summary>
        /// <returns></returns>
        public (string[] columns, MemberInfo[] props) GetMeta()
        {
            return (Queue.Select(s => s.columnName).ToArray(), Queue.Select(s => s.member).ToArray());
        }

        /// <summary>
        /// Container has columns
        /// </summary>
        public bool HasColumn => Queue.Any();

        /// <summary>
        /// Select and set the name for first members 
        /// </summary>
        /// <param name="names">Property names</param>
        /// <returns>Current container</returns>
        internal ColumnSequenceContainer<T> TupleRename(params string[] names)
        {
            var members = typeof(T).GetSuportedMembers();
            for (int i = 0; i < names.Length; i++)
            {
                Queue.Enqueue((names[i], members[i]));
            }

            return this;
        }

        /// <summary>
        /// Add property or field to container
        /// </summary>
        /// <param name="expression">Add property or field to out table</param>
        /// <param name="name">Column name</param>
        /// <returns>Current container</returns>
        public ColumnSequenceContainer<T> Add<TProp>(Expression<Func<T, TProp>> expression, string name = null)
        {
            var expBody = expression.Body as MemberExpression;

            if (expBody != null &&
                        (expBody.Member.MemberType == MemberTypes.Field ||
                            expBody.Member.MemberType == MemberTypes.Property))
            {
                Queue.Enqueue((string.IsNullOrWhiteSpace(name) ? expBody.Member.Name: name, expBody.Member));
            }
            else
            {
                throw new ArgumentException($"Expression ({expression.Body}) must return field or property", nameof(expression));
            }

            return this;
        }
    }
}