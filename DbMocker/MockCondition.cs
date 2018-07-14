using System;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockCondition
    {
        internal Func<MockCommand, MockTable> ReturnsFunction { get; set; }

        /// <summary />
        internal Func<MockCommand, bool> Condition { get; set; }

        /// <summary />
        public void ReturnsTable(Func<MockCommand, MockTable> returns)
        {
            ReturnsFunction = returns;
        }

        /// <summary />
        public void ReturnsTable(MockTable returns)
        {
            ReturnsFunction = (cmd) => returns;
        }

        /// <summary />
        public void ReturnsScalar<T>(Func<MockCommand, T> returns)
        {
            ReturnsFunction = (cmd) => new MockTable()
            {
                Columns = new[] { String.Empty },
                Rows = new object[,]
                    {
                        { returns.Invoke(cmd) }
                    }
            };
        }

        /// <summary />
        public void ReturnsScalar<T>(T returns)
        {
            ReturnsFunction = (cmd) => new MockTable()
            {
                Columns = new[] { String.Empty },
                Rows = new object[,]
                    {
                        { returns }
                    }
            };
        }

    }
}
