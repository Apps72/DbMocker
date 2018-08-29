using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Data.Common;
using System.Collections.Generic;

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

        // Sample method from your DataService
        public object[][] GetEmployees(DbConnection connection)
        {            
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT ID, Name FROM Employees";
                var reader = cmd.ExecuteReader();

                var data = new List<object[]>();
                while (reader.Read())
                {
                    var row = new object[reader.FieldCount];
                    reader.GetValues(row);
                    data.Add(row);

                    int id = reader.GetValue(0) == null ? 0 : reader.GetInt32(0);
                    string name = reader.GetString(1);

                }

                return data.ToArray();
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

        [TestMethod]
        public void UnitTest2()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(new MockTable().AddColumns("ID", "Name")
                                             .AddRow(1, "Scott")
                                             .AddRow(2, "Bill"));

            var data = GetEmployees(conn);

            Assert.AreEqual("Scott", data[0][1]);
        }

        [TestMethod]
        public void UnitTest3()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(MockTable.Empty()
                                       .AddColumns("ID", "Name")
                                       .AddRow(1, "Scott"));

            var data = GetEmployees(conn);

            Assert.AreEqual("Scott", data[0][1]);
        }

        [TestMethod]
        public void UnitTest4()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(MockTable.SingleCell("Count", 14));

            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(14, count);
        }

        [TestMethod]
        public void UnitTest5()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(cmd => MockTable.SingleCell("Count", cmd.Parameters.Count()));

            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void UnitTest6()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsScalar<int>(14);

            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(14, count);
        }

        [TestMethod]
        public void UnitTest7()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(MockTable.Empty()
                                       .AddColumns("ID", "Name")
                                       //.AddColumns(("ID", typeof(int?)), 
                                       //            ("Name", typeof(string)))
                                       .AddRow(null, null)
                                       .AddRow(1, "Scott"));

            var data = GetEmployees(conn);

            Assert.AreEqual("Scott", data[1][1]);
        }

    }
}
