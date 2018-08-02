using System;
using System.Collections.Generic;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockConditions
    {
        /// <summary />
        internal MockConditions(MockDbConnection connection)
        {

        }

        /// <summary />
        internal List<MockReturns> Conditions = new List<MockReturns>();

        /// <summary />
        public MockReturns When(Func<MockCommand, bool> condition)
        {
            var mock = new MockReturns() { Condition = condition };
            Conditions.Add(mock);
            return mock;
        }

        /// <summary />
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
