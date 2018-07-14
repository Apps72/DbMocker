using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockManager
    {
        /// <summary />
        internal MockManager(MockDbConnection connection)
        {

        }

        /// <summary />
        internal List<MockCondition> Conditions = new List<MockCondition>();

        /// <summary />
        public MockCondition When(Func<MockCommand, bool> condition)
        {
            var mock = new MockCondition() { Condition = condition };
            Conditions.Add(mock);
            return mock;
        }

        /// <summary />
        public MockCondition WhenAlways()
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
