using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary />
    public class MockCommand
    {
        /// <summary />
        internal MockCommand(MockDbCommand command)
        {
            this.CommandText = command.CommandText;
            this.Parameters = command.Parameters.Cast<DbParameter>();
        }

        /// <summary />
        public string CommandText { get; set; }

        /// <summary />
        public IEnumerable<DbParameter> Parameters { get; set; }
    }
}
