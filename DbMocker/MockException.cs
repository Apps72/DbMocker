using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker
{
    /// <summary>
    /// Exception raised when a Mock <see cref="MockConditions"/> is not defined.
    /// </summary>
    public class MockException : ApplicationException
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ApplicationException class.
        public MockException() 
            : base() { }

        //
        // Summary:
        //     Initializes a new instance of the System.ApplicationException class with a specified
        //     error message.
        //
        // Parameters:
        //   message:
        //     A message that describes the error.
        public MockException(string message)
            : base(message) { }

        //
        // Summary:
        //     Initializes a new instance of the System.ApplicationException class with a specified
        //     error message and a reference to the inner exception that is the cause of this
        //     exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not a null reference, the current exception is raised in a catch
        //     block that handles the inner exception.
        public MockException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// SQL Query which generate the exception.
        /// </summary>
        public string CommandText { get; internal set; }

        /// <summary>
        /// List of parameters used by the SQL Query which generate the exception.
        /// </summary>
        public IEnumerable<DbParameter> Parameters { get; internal set; }
    }
}
