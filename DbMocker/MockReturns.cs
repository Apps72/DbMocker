using Apps72.Dev.Data.DbMocker.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    [System.Diagnostics.DebuggerDisplay("{Description}")]
    public class MockReturns
    {
        /// <summary>
        /// Gets the Description of this condition.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary />
        private List<Action<MockCommand>> _modifyParameter = new List<Action<MockCommand>>();

        private Func<MockCommand, MockTable[]> _returnsFunction;

        private readonly List<Action<MockCommand>> _callbacks = new List<Action<MockCommand>>();

        /// <summary />
        internal Func<MockCommand, MockTable[]> ReturnsFunction
        {
            get
            {
                return (command) =>
                {
                    foreach (Action<MockCommand> callback in _callbacks)
                    {
                        callback(command);
                    }

                    foreach (var mody in _modifyParameter)
                    {
                        mody(command);
                    }

                    return _returnsFunction(command);
                };
            }
            set
            {
                _returnsFunction = value;
            }
        }

        /// <summary />
        internal Func<MockCommand, bool> Condition { get; set; }

        /// <summary>
        /// Sets the expression to return a multiple result set, presented by an array of
        /// <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns">Method or Lambda expression to return a <seealso cref="MockTable"/></param>
        public void ReturnsDataset(params MockTable[] returns)
        {
            ReturnsFunction = (cmd) => returns;
        }

        /// <summary>
        /// Sets the expression to return a multiple result set, presented by an array of
        /// <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns">Method or Lambda expression to return a <seealso cref="MockTable"/></param>
        public void ReturnsDataset(Func<MockCommand, MockTable[]> returns)
        {
            ReturnsFunction = returns;
        }

        /// <summary>
        /// Sets the expression to return a <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns">Method or Lambda expression to return a <seealso cref="MockTable"/></param>
        public void ReturnsTable(Func<MockCommand, MockTable> returns)
        {
            ReturnsFunction = (cmd) => new[] { returns.Invoke(cmd) };
        }

        /// <summary>
        /// Sets <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns"><seealso cref="MockTable"/> with sample data</param>
        public void ReturnsTable(MockTable returns)
        {
            ReturnsFunction = (cmd) => new[] { returns };
        }

        /// <summary>
        /// Sets the expression to return a sample data object, when the condition occured.
        /// This object is converted to a <seealso cref="MockTable"/> where columns are property names
        /// and data in the first row are proerty values.
        /// </summary>
        /// <param name="returns">Sample data</param>
        public void ReturnsRow<T>(Func<MockCommand, T> returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTables(returns.Invoke(cmd));
        }

        /// <summary>
        /// Sets a sample data object, when the condition occured.
        /// This object is converted to a <seealso cref="MockTable"/> where columns are property names
        /// and data in the first row are proerty values.
        /// </summary>
        /// <param name="returns">Sample data</param>
        public void ReturnsRow<T>(T returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTables(returns);
        }

        /// <summary>
        /// Sets <seealso cref="MockTable"/> with sample data, when the condition occured.
        /// </summary>
        /// <param name="returns"><seealso cref="MockTable"/> with sample data</param>
        public void ReturnsRow(MockTable returns)
        {
            ReturnsFunction = (cmd) => new[] { returns };
        }

        /// <summary>
        /// Throws the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void ThrowsException(Exception exception)
        {
            ReturnsFunction = command => throw exception;
        }

        /// <summary>
        /// Throws the specified exception type.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        public void ThrowsException<TException>() where TException : Exception, new()
        {
            ReturnsFunction = command => throw new TException();
        }

        /// <summary>
        /// Set output value for dbParameter, when the condition occured.
        /// </summary>
        /// <param name="returns">Value to return</param>
        public MockReturns SetParameterValue(string parameterName, object value, ParameterDirection direction = ParameterDirection.Output)
        {
            _modifyParameter.Add((command) =>
            {
                var parameter = command.Parameters.FirstOrDefault(a => a.ParameterName == parameterName);

                if (parameter == null)
                {
                    throw new MockException($"parameterName '{parameterName}' doesn't find");
                }

                if (parameter.Direction != direction)
                {
                    throw new MockException($"Actual direction parameter '{parameter.Direction.ToString()}' doesn't equels expected value '{direction}'");
                }

                parameter.Value = value;
            });

            return this;
        }

        /// <summary>
        /// Sets the expression to execute an action, to trace some data for example.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public MockReturns Callback(Action<MockCommand> callback)
        {
            if (callback != null)
            {
                _callbacks.Add(callback);
            }

            return this;
        }

        /// <summary>
        /// Sets the expression to return the value, when the condition occured.
        /// </summary>
        /// <param name="returns">Value to return</param>
        public void ReturnsScalar<T>(Func<MockCommand, T> returns)
        {
            if (returns == null)
            {
                ReturnsFunction = (cmd) => ConvertToMockTables(returns);
            }
            else
            {
                ReturnsFunction = (cmd) => ConvertToMockTables(returns.Invoke(cmd));
            }
        }

        /// <summary>
        /// Sets the value to return when the condition occured.
        /// </summary>
        /// <param name="returns">Value to return</param>
        public void ReturnsScalar<T>(T returns)
        {
            ReturnsFunction = (cmd) => ConvertToMockTables(returns);
        }

        /// <summary />
        private MockTable[] ConvertToMockTables<T>(T returns)
        {
            return new[] { ConvertToMockTable(returns) };
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
                        { returns }
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
