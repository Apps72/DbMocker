using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockConditions
    {
        internal bool MustValidateSqlServerCommandText = false;
        private static readonly string NEW_LINE = Environment.NewLine;
        private List<MockReturns> _conditions = new List<MockReturns>();

        /// <summary />
        internal MockConditions(MockDbConnection connection)
        {

        }

        /// <summary>
        /// Gets all conditions recorded.
        /// </summary>
        public IEnumerable<MockReturns> Conditions => _conditions;

        /// <summary>
        /// Add a condition to return mock data.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MockReturns When(Func<MockCommand, bool> condition)
        {
            return WhenPrivate($"When([Expression])", condition);
        }

        /// <summary>
        /// Add a condition to return mock data, when the <paramref name="tagName"/> is detected.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MockReturns WhenTag(string tagName)
        {
            return WhenPrivate(
                description: $"WhenTag({tagName})",
                condition: (cmd) =>
                {
                    string toSearch = $"-- {tagName}{NEW_LINE}";
                    return cmd.CommandText.StartsWith(toSearch) ||
                           cmd.CommandText.Contains($"{NEW_LINE}{toSearch}");
                });
        }

        /// <summary>
        /// Load all Embedded resource "[TagName].txt" with Fixed format.
        /// And add it as <see cref="WhenTag(string)"/> method.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="tagNames"></param>
        public void LoadTagsFromResources(params string[] tagNames)
        {
            this.LoadTagsFromResources(Assembly.GetCallingAssembly(), tagNames);
        }

        /// <summary>
        /// Load all Embedded resource "[TagName].txt" with Fixed format.
        /// And add it as <see cref="WhenTag(string)"/> method.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="tagNames"></param>
        public void LoadTagsFromResources(Assembly assembly, params string[] tagNames)
        {
            string[] allResources = assembly.GetManifestResourceNames();

            foreach (var tag in tagNames)
            {
                string resourceName = allResources.FirstOrDefault(i => i.EndsWith($"{tag}.txt", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(resourceName))
                {
                    var table = MockTable.FromFixed(assembly, resourceName);
                    this.WhenTag(tag)
                        .ReturnsTable(table);
                }
            }
        }

        /// <summary>
        /// Catch all SQL queries to returns always the same mock data.
        /// </summary>
        /// <returns></returns>
        public MockReturns WhenAny()
        {
            return WhenPrivate("WhenAny()", null);
        }

        /// <summary>
        /// Check if queries have correct SQL Server syntax.
        /// </summary>
        /// <returns></returns>
        public MockConditions HasValidSqlServerCommandText()
        {
            return HasValidSqlServerCommandText(toValidate: true);
        }

        /// <summary>
        /// Check if queries have correct SQL Server syntax.
        /// </summary>
        /// <param name="toValidate">To validate or not, the SQL queries.</param>
        /// <returns></returns>
        public MockConditions HasValidSqlServerCommandText(bool toValidate)
        {
            MustValidateSqlServerCommandText = toValidate;
            return this;
        }

        /// <summary />
        internal MockTable[] GetFirstMockTablesFound(MockCommand command)
        {
            foreach (MockReturns item in _conditions)
            {
                if (item.Condition.Invoke(command) == true)
                {
                    return item.ReturnsFunction(command);
                }
            }

            throw new MockException("No mock found. Use MockDbConnection.Mocks.Where(...).Returns(...) methods to define mocks.")
            {
                CommandText = command.CommandText,
                Parameters = command.Parameters
            };
        }

        /// <summary>
        /// Add a condition to return mock data.
        /// </summary>
        /// <param name="description">Label to identify the condition</param>
        /// <param name="condition">Function to execute</param>
        /// <returns></returns>
        private MockReturns WhenPrivate(string description, Func<MockCommand, bool> condition)
        {
            if (condition == null)
            {
                condition = (cmd => true);
            }

            var mock = new MockReturns()
            {
                Description = description,
                Condition = (cmd) =>
                {
                    if (MustValidateSqlServerCommandText)
                    {
                        return condition.Invoke(cmd) && cmd.HasValidSqlServerCommandText();
                    }
                    else
                    {
                        return condition.Invoke(cmd);
                    }
                }
            };
            _conditions.Add(mock);
            return mock;
        }

    }
}
