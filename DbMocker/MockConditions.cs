using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockConditions
    {
        private static readonly string NEW_LINE = Environment.NewLine;
        internal bool MustValidateSqlServerCommandText = false;

        /// <summary />
        internal MockConditions(MockDbConnection connection)
        {
            
        }

        /// <summary />
        internal List<MockReturns> Conditions = new List<MockReturns>();

        /// <summary>
        /// Add a condition to return mock data.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MockReturns When(Func<MockCommand, bool> condition)
        {
            if (condition == null) condition = (cmd => true);

            var mock = new MockReturns()
            {
                Condition = (cmd) => {
                    if (MustValidateSqlServerCommandText)
                        return condition.Invoke(cmd) && cmd.HasValidSqlServerCommandText();
                    else
                        return condition.Invoke(cmd);
                                     }
            };
            Conditions.Add(mock);
            return mock;
        }

        /// <summary>
        /// Add a condition to return mock data, when the <paramref name="tagName"/> is detected.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public MockReturns WhenTag(string tagName)
        {
            return When((cmd) =>
                {
                    string toSearch = $"-- {tagName}{NEW_LINE}";
                    return cmd.CommandText.StartsWith(toSearch) ||
                           cmd.CommandText.Contains($"{NEW_LINE}{toSearch}");
                });
        }

        /// <summary>
        /// Catch all SQL queries to returns always the same mock data.
        /// </summary>
        /// <returns></returns>
        public MockReturns WhenAny()
        {
            return When(null);
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
        internal MockTable GetFirstMockTableFound(MockCommand command)
        {
            foreach (var item in this.Conditions)
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
    }
}
