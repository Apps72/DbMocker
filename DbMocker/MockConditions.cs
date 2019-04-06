using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockConditions
    {
        private static readonly string NEW_LINE = Environment.NewLine;

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
            var mock = new MockReturns() { Condition = condition };
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
            var mock = new MockReturns()
            {
                Condition = (cmd) => 
                {
                    string toSearch = $"-- {tagName}{NEW_LINE}";
                    return cmd.CommandText.StartsWith(toSearch) || 
                           cmd.CommandText.Contains($"{NEW_LINE}{toSearch}");
                }
            };
            Conditions.Add(mock);
            return mock;
        }

        /// <summary>
        /// Catch all SQL queries to returns always the same mock data.
        /// </summary>
        /// <returns></returns>
        public MockReturns WhenAny()
        {
            return When(null);
        }

        /// <summary />
        internal MockTable GetFirstMockTableFound(MockCommand command)
        {
            foreach (var item in this.Conditions)
            {
                if (item.Condition == null || item.Condition?.Invoke(command) == true)
                {
                    return item.ReturnsFunction(command);
                }
            }

            throw new ArgumentException("No mock found. Use MockDbConnection.Mocks.Where(...).Returns(...) methods to define mocks.");
        }
    }
}
