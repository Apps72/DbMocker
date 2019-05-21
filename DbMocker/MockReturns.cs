using Apps72.Dev.Data.DbMocker.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockReturns
    {
        /// <summary />
        internal Func<MockCommand, MockTable> ReturnsFunction { get; set; }

        /// <summary />
        internal Func<MockCommand, bool> Condition { get; set; }

        /// <summary>
        /// Sets the expression to return a <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns">Method or Lambda expression to return a <seealso cref="MockTable"/></param>
        public void ReturnsTable(Func<MockCommand, MockTable> returns)
        {
            ReturnsFunction = returns;
        }

        /// <summary>
        /// Sets <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns"><seealso cref="MockTable"/> with sample data</param>
        public void ReturnsTable(MockTable returns)
        {
            ReturnsFunction = (cmd) => returns;
        }

        /// <summary>
        /// Sets the expression to return a sample data object, when the condition occured.
        /// This object is converted to a <seealso cref="MockTable"/> where columns are property names 
        /// and data in the first row are proerty values.
        /// </summary>
        /// <param name="returns">Sample data</param>
        public void ReturnsRow<T>(Func<MockCommand, T> returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTable(returns.Invoke(cmd));
        }

        /// <summary>
        /// Sets a sample data object, when the condition occured.
        /// This object is converted to a <seealso cref="MockTable"/> where columns are property names 
        /// and data in the first row are proerty values.
        /// </summary>
        /// <param name="returns">Sample data</param>
        public void ReturnsRow<T>(T returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTable(returns);
        }

        /// <summary>
        /// Sets <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns"><seealso cref="MockTable"/> with sample data</param>
        public void ReturnsRow(MockTable returns)
        {
            ReturnsFunction = (cmd) => returns;
        }

        /// <summary>
        /// Sets the expression to return the value, when the condition occured.
        /// </summary>
        /// <param name="returns">Value to return</param>
        public void ReturnsScalar<T>(Func<MockCommand, T> returns)
        {
            if (returns == null)
            {
                ReturnsFunction = (cmd) => ConvertToMockTable(returns);
            }
            else
            {
                ReturnsFunction = (cmd) => ConvertToMockTable(returns.Invoke(cmd));
            }
        }

        /// <summary>
        /// Sets the value to return when the condition occured.
        /// </summary>
        /// <param name="returns">Value to return</param>
        public void ReturnsScalar<T>(T returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTable(returns);
        }

        /// <summary />
        private MockTable ConvertToMockTable<T>(T returns)
        {
            if (returns == null || returns is DBNull || typeof(T).IsPrimitive())
            {
                return new MockTable()
                {
                    Columns = Columns.WithNames(string.Empty),
                    Rows = new object[,]
                    {
                        { GetValueOrDbNull(returns) }
                    }
                };
            }
            else
            {
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var columns = new List<string>();
                var values = new List<object>();

                foreach (PropertyInfo property in properties)
                {
                    columns.Add(property.Name);
                    values.Add(GetValueOrDbNull(property.GetValue(returns)));
                }

                return new MockTable(columns.ToArray(),
                                     new object[][] { values.ToArray() }.ToTwoDimensionalArray());
            }
        }

        /// <summary />
        private object GetValueOrDbNull(object value)
        {
            return value == null ? DBNull.Value : value;
        }
    }
}
