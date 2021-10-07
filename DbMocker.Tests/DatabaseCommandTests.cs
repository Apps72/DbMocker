using Apps72.Dev.Data;
using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DatabaseCommandTests
    {
        [TestMethod]
        public void Mock_ContainsSql_IntegerScalar_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT * FROM EMP WHERE ID = @ID");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_IntegerScalar_Null_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar((int?)null);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(0, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_NullableIntegerScalar_Null_Test()
        {
            // Issue #27 - https://github.com/Apps72/DbMocker/issues/27
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbcommand.executescalar
            //   If the first column of the first row in the result set is not found, a null reference is returned.

            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar((int?)null);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");

                var result = cmd.ExecuteScalar();

                Assert.AreEqual(null, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_NullableIntegerScalar_DbNull_Test()
        {
            // Issue #27 - https://github.com/Apps72/DbMocker/issues/27
            // https://docs.microsoft.com/en-us/dotnet/api/system.data.common.dbcommand.executescalar
            //   If the value in the database is null, the query returns DBNull.Value.

            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(DBNull.Value);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteScalar();

                Assert.AreEqual(DBNull.Value, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_IntegerScalar_DbNull_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(System.DBNull.Value);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(0, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_StringScalar_DbNull_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(System.DBNull.Value);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteScalar<string>();

                Assert.AreEqual(null, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_StringScalar_Null_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar((string)null);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteScalar<string>();

                Assert.AreEqual(null, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_And_Parameter_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT") &&
                           c.Parameters.Any(p => p.ParameterName == "@ID"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ExecuteNonQuery_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("INSERT"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("INSERT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteNonQuery();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ExecuteTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(null)
                .ReturnsTable(new MockTable()
                {
                    Columns = Columns.WithNames("Col1", "Col2", "Col3"),
                    Rows = new object[,]
                    {
                        { 0,      1,      2 },
                        { 9,      8,      7 },
                        { 4,      5,      6 },
                    }
                });

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteTable(new
                {
                    Col1 = 0,
                    Col2 = 0,
                    Col3 = 0
                });

                Assert.AreEqual(3, result.Count());          // 3 rows
                Assert.AreEqual(1, result.First().Col2);     // First row / Col2
            }

        }

        [TestMethod]
        public void Mock_ParameterOutput_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("EXEC PROC"))
                .SetParameterValue("@OUT", "OutValue")
                .ReturnsScalar(777);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC PROC @out1 = @OUT";

            var outParam = cmd.CreateParameter();
            outParam.ParameterName = "@OUT";
            outParam.Direction = System.Data.ParameterDirection.Output;
            cmd.Parameters.Add(outParam);

            var result = cmd.ExecuteScalar();

            Assert.AreEqual("OutValue", outParam.Value);
        }

        [TestMethod]
        public void Mock_ExecuteTable_WithNullInFirstRow_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(null)
                .ReturnsTable(new MockTable()
                {
                    Columns = Columns.WithNames("Col1", "Col2", "Col3"),
                    Rows = new object[,]
                    {
                        { null,   1,      2 },
                        { null,   8,      7 },
                        { 4,      5,      6 },
                    }
                });

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteTable(new
                {
                    Col1 = (int?)0,
                    Col2 = 0,
                    Col3 = 0
                });

                Assert.AreEqual(null, result.ElementAt(0).Col1);
                Assert.AreEqual(null, result.ElementAt(1).Col1);
                Assert.AreEqual(4, result.ElementAt(2).Col1);
            }

        }

        [TestMethod]
        public void Mock_ExecuteRow_WithTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(
                      MockTable.WithColumns("Col1", "Col2")
                               .AddRow(10, 11)
                               .AddRow(12, 13));

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteRow(new
                {
                    Col1 = default(int?),
                    Col2 = default(int),
                });

                Assert.AreEqual(10, result.Col1);
                Assert.AreEqual(11, result.Col2);
            }

        }

        [TestMethod]
        public void Mock_ExecuteScalar_WithTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(
                      new MockTable().AddColumns("Col1", "Col2")
                                     .AddRow(10, 11)
                                     .AddRow(12, 13));

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteRow<int>();

                Assert.AreEqual(10, result);
            }

        }

        [TestMethod]
        public void Mock_FormatedCommandText_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText = @"SELECT * 
                                      FROM EMP 
                                     WHERE ID = @Id
                                       AND ENAME = @Name 
                                       AND HIREDATE = @HireDate";

                cmd.AddParameter("@Id", 123);
                cmd.AddParameter("@Name", "Denis");
                cmd.AddParameter("@HireDate", new DateTime(2019, 05, 03));

                var formatedCommandAsText = cmd.Formatted.CommandAsText;
                var formatedCommandAsVariables = cmd.Formatted.CommandAsVariables;

                Assert.IsTrue(formatedCommandAsText.Contains("ID = 123"));
                Assert.IsTrue(formatedCommandAsText.Contains("ENAME = 'Denis'"));
                Assert.IsTrue(formatedCommandAsText.Contains("HIREDATE = '2019-05-03'"));

                Assert.IsTrue(formatedCommandAsVariables.Contains("DECLARE @Id AS INT = 123"));
                Assert.IsTrue(formatedCommandAsVariables.Contains("DECLARE @Name AS VARCHAR"));
                Assert.IsTrue(formatedCommandAsVariables.Contains("DECLARE @HireDate AS DATETIME = '2019-05-03'"));
            }
        }

        [TestMethod]
        public void Mock_ExecuteDataset_Test()
        {
            var conn = new MockDbConnection();

            MockTable table1 = MockTable.WithColumns("Col1", "Col2")
                                        .AddRow(11, 12);

            MockTable table2 = MockTable.WithColumns("Col3", "Col4")
                                        .AddRow("MyString", 3.4);

            conn.Mocks
                .When(null)
                .ReturnsDataset(table1, table2);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteDataSet(new { Col1 = 0, Col2 = 0 },
                                                new { Col3 = "", Col4 = 0.0 });

                Assert.AreEqual(1, result.Item1.Count());          // 1 row
                Assert.AreEqual(11, result.Item1.First().Col1);
                Assert.AreEqual(12, result.Item1.First().Col2);

                Assert.AreEqual(1, result.Item2.Count());          // 1 row
                Assert.AreEqual("MyString", result.Item2.First().Col3);
                Assert.AreEqual(3.4, result.Item2.First().Col4);
            }

        }

        [TestMethod]
        public void Mock_ExecuteDataset_WithFunction_Test()
        {
            var conn = new MockDbConnection();

            MockTable table1 = MockTable.WithColumns("Col1", "Col2")
                                        .AddRow(11, 12);

            MockTable table2 = MockTable.WithColumns("Col3", "Col4")
                                        .AddRow("MyString", 3.4);

            conn.Mocks
                .When(null)
                .ReturnsDataset(cmd => new[] { table1, table2 });

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                var result = cmd.ExecuteDataSet(new { Col1 = 0, Col2 = 0 },
                                                new { Col3 = "", Col4 = 0.0 });

                Assert.AreEqual(1, result.Item1.Count());          // 1 row
                Assert.AreEqual(11, result.Item1.First().Col1);
                Assert.AreEqual(12, result.Item1.First().Col2);

                Assert.AreEqual(1, result.Item2.Count());          // 1 row
                Assert.AreEqual("MyString", result.Item2.First().Col3);
                Assert.AreEqual(3.4, result.Item2.First().Col4);
            }

        }

        [TestMethod]
        public void Mock_ThrowException_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ThrowsException(new Exception("TestException"));

            using (var cmd = new DatabaseCommand(conn))
            {
                var exception = Assert.ThrowsException<Exception>(() =>
                {
                    cmd.CommandText.AppendLine("SELECT ...");
                    cmd.ExecuteScalar<int>();
                });

                Assert.IsNotNull(exception);
                Assert.AreEqual("TestException", exception.Message);
            }
        }

        [TestMethod]
        public void Mock_ThrowGenericException_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ThrowsException<Exception>();

            using (var cmd = new DatabaseCommand(conn))
            {
                var exception = Assert.ThrowsException<Exception>(() =>
                {
                    cmd.CommandText.AppendLine("SELECT ...");
                    cmd.ExecuteScalar<int>();
                });

                Assert.IsNotNull(exception);
            }
        }

    }
}
