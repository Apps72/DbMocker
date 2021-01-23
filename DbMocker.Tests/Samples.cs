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
        [TestMethod]
        public void UnitTest0()
        {
            var conn = new MockDbConnection();

            // The text file "123-EMPLOYEES.txt" is embedded in this project.
            // See the Samples folder.
            //  - 123       is an identifier (as you want)
            //  - EMPLOYEES is the CommandText Tag 
            //    See https://docs.microsoft.com/en-us/ef/core/querying/tags
            conn.Mocks.LoadTagsFromResources("123-EMPLOYEES");

            // Call your "classic" methods to tests
            var data = GetEmployees(conn);

            // DbMocker read the embedded file 
            // and associated the content to the tag
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual("Scott", data[0][1]);
            Assert.AreEqual("Bill", data[1][1]);
        }

        // Sample method from your DataService
        public int GetNumberOfEmployees(DbConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"-- COUNT_EMPLOYEES
                                    SELECT COUNT(*) FROM Employees";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Sample method from your DataService
        public object[][] GetEmployees(DbConnection connection)
        {
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"-- EMPLOYEES
                                    SELECT ID, Name FROM Employees";
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
                .When(cmd => cmd.CommandText.Contains("COUNT_EMPLOYEES") &&
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
                .WhenTag("EMPLOYEES")
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
                .WhenTag("EMPLOYEES")
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
                .WhenTag("COUNT_EMPLOYEES")
                .ReturnsTable(MockTable.SingleCell("Count", 14));

            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(14, count);
        }

        [TestMethod]
        public void UnitTest5()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenTag("COUNT_EMPLOYEES")
                .ReturnsTable(cmd => MockTable.SingleCell("Count", cmd.Parameters.Count()));

            int count = GetNumberOfEmployees(conn);

            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void UnitTest6()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenTag("COUNT_EMPLOYEES")
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
                .ReturnsTable(MockTable.WithColumns(("ID", typeof(int?)),
                                                    ("Name", typeof(string)))
                                        .AddRow(null, "Scott")
                                        .AddRow(2, "Bill"));

            var data = GetEmployees(conn);

            Assert.AreEqual(null, data[0][0]);
            Assert.AreEqual("Scott", data[0][1]);
        }

        [TestMethod]
        public void UnitTest8()
        {
            var conn = new MockDbConnection();

            // Sample data rows
            var samples = new[]
                {
                    new { ID = 0, Name = "Scott" },
                    new { ID = 1, Name = "Denis" },
                };

            conn.Mocks
                .WhenTag("EMPLOYEES")
                .ReturnsTable(MockTable.FromType(samples));

            var data = GetEmployees(conn);

            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(0, data[0][0]);
            Assert.AreEqual("Scott", data[0][1]);
        }


    }
}
