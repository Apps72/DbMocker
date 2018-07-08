using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

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
        internal object GetFirstReturnsFound(MockCommand command)
        {
            foreach (var item in this.Conditions)
            {
                if (item.Condition.Invoke(command))
                {
                    return item.GetReturnsValue(command);
                }
            }

            throw new ArgumentException("No mock found. Use MockDbConnection.Mocks.Where(...).Returns(...) methods to define mocks.");
        }
    }    
}
