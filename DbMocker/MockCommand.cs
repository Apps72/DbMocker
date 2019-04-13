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
        internal MockCommand(Data.MockDbCommand command)
        {
            this.CommandText = command.CommandText;
            this.Parameters = command.Parameters.Cast<DbParameter>();
        }

        /// <summary />
        public string CommandText { get; set; }

        /// <summary />
        public IEnumerable<DbParameter> Parameters { get; set; }

        /// <summary />
        public bool HasValidSqlServerCommandText()
        {
            var parseErrors = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(this.CommandText);
            var hasError = parseErrors.Errors.Any(err => err.IsWarning == false);
            if (hasError)
            {
                var errorStrings = parseErrors.Errors
                                              .Where(err => err.IsWarning == false)
                                              .Select(err => $"{err.Start}. {err.Message}");

                var innerException = new Exception(String.Join(Environment.NewLine, errorStrings));

                throw new MockException("DbMocker failed. The CommandText is not a valid SQL Server query.",
                                        innerException)
                {
                    CommandText = this.CommandText,
                    Parameters = this.Parameters,
                };
            }
            else
            {
                return true;
            }

        }
    }
}
