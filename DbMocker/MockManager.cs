using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    public class MockManager
    {
        internal MockManager(MockDbConnection connection)
        {
            
        }

        internal List<MockCondition> Conditions = new List<MockCondition>();

        public MockCondition When(Func<MockCommand, bool> condition)
        {
            var mock = new MockCondition() { Condition = condition };
            Conditions.Add(mock);
            return mock;
        }

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

    public class MockCondition
    {
        private object returnsValue = null;
        private Func<MockCommand, object> returnsFunction = null;

        internal Func<MockCommand, bool> Condition { get; set; }
        
        public void Returns(Func<MockCommand, object> returns)
        {
            returnsFunction = returns;
        }

        public void Returns(object returns)
        {
            returnsValue = returns;
        }

        internal object GetReturnsValue(MockCommand command)
        {
            if (returnsFunction != null)
                return returnsFunction.Invoke(command);
            else
                return returnsValue;

        }
    }

    public class MockCommand
    {
        internal MockCommand(MockDbCommand command)
        {
            this.CommandText = command.CommandText;
            this.Parameters = command.Parameters.Cast<DbParameter>();
        }

        public string CommandText { get; set; }
        public IEnumerable<DbParameter> Parameters { get; set; }
    }
}
