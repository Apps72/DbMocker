using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using Apps72.Dev.Data.DbMocker;
using System.Data.Common;

namespace DbMocker.Tests
{
    [TestClass]
    public class Samples
    {
        // Sample method from your DataService
        public int GetNumberOfEmployees(DbConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Employees";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        [TestMethod]
        public void UnitTest1()
        {
            var conn = new MockDbConnection();

            // When a specific SQL command is detected,
            // Don't execute the query to your SQL Server,
            // But returns this MockTable.
            conn.Mocks
                .When(cmd => cmd.CommandText.StartsWith("SELECT COUNT(*)") &&
                                cmd.Parameters.Count() == 0)
                .ReturnsTable(MockTable.WithColumns("Count")
                                        .AddRow(14));

            // Call your "classic" methods to tests
            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(14, count);
        }
    }
}
