using System;
using System.Collections.Generic;
using System.Text;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockCondition
    {
        private object returnsValue = null;
        private Func<MockCommand, object> returnsFunction = null;

        /// <summary />
        internal Func<MockCommand, bool> Condition { get; set; }

        /// <summary />
        public void Returns(Func<MockCommand, object> returns)
        {
            returnsFunction = returns;
        }

        /// <summary />
        public void Returns(object returns)
        {
            returnsValue = returns;
        }

        /// <summary />
        public void ReturnsScalar<T>(Func<MockCommand, T> returns)
        {
            returnsFunction = (MockCommand cmd) => returns.Invoke(cmd);
        }

        /// <summary />
        public void ReturnsScalar<T>(T returns)
        {
            returnsValue = returns;
        }

        /// <summary />
        internal object GetReturnsValue(MockCommand command)
        {
            if (returnsFunction != null)
                return returnsFunction.Invoke(command);
            else
                return returnsValue;

        }
    }
}
