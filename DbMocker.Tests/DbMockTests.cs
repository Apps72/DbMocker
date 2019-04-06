using Apps72.Dev.Data.DbMocker;
using Apps72.Dev.Data.DbMocker.Data;
using Apps72.Dev.Data.DbMocker.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTests
    {
        [TestMethod]
        public void Mock_CheckTwoDimensionalObject_Test()
        {
            var x = new object[,] 
            {
                { "Col1", "Col2" },
                { "abc" , "def" },
                { "ghi" , "jkl" }
            };
            var y = new object[,]
            {
                { },
            };
            var z = new object[,] 
            {
            };

            Assert.AreEqual(2, x.Rank);
            Assert.AreEqual(6, x.Length);
            Assert.AreEqual(3, x.GetLength(0));
            Assert.AreEqual(2, x.GetLength(1));
            Assert.AreEqual("Col1", x[0, 0]);
            Assert.AreEqual("Col2", x[0, 1]);

            Assert.AreEqual(2, y.Rank);
            Assert.AreEqual(0, y.Length);
            Assert.AreEqual(1, y.GetLength(0));
            Assert.AreEqual(0, y.GetLength(1));

            Assert.AreEqual(2, z.Rank);
            Assert.AreEqual(0, z.Length);
            Assert.AreEqual(0, z.GetLength(0));
            Assert.AreEqual(0, z.GetLength(1));
        }

        [TestMethod]
        public void Mock_ConvertToJaggedArray_Test()
        {
            var source = new object[,]
            {
                { "Col1", "Col2" },
                { "abc" , "def" },
                { "ghi" , "jkl" }
            };

            var target = source.ToJaggedArray();

            Assert.AreEqual("Col1", target[0][0]);
            Assert.AreEqual("def", target[1][1]);
            Assert.AreEqual("ghi", target[2][0]);
        }

        [TestMethod]
        public void Mock_ConvertToTwoDimensionalArray_Test()
        {
            var source = new object[][]
            {
                new [] { "Col1", "Col2" },
                new [] { "abc" , "def" },
                new [] { "ghi" , "jkl" }
            };

            var target = source.ToTwoDimensionalArray();

            Assert.AreEqual("Col1", target[0, 0]);
            Assert.AreEqual("def", target[1, 1]);
            Assert.AreEqual("ghi", target[2, 0]);
        }

        [TestMethod]
        public void Mock_ReturnsMockTable_Properties_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsTable(new MockTable()
                {
                    Columns = Columns.WithNames("X"),
                    Rows = new object[,] 
                    {
                        { 14 }
                    }
                });

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsMockTable_Constructor_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsTable(new MockTable(columns: new[] { "X" },
                                       rows: new object[,]
                                            {
                                                { 14 }
                                            }));
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsEmptyTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsTable(new MockTable());

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void Mock_MockNotFound_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("NOT PRESENT"))
                .ReturnsScalar(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();
        }

        [TestMethod]
        public void Mock_ReturnsRow_ExpressionTo_TypedData_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow((c) => new { Id = 11, Name = "Denis" });

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual("Denis", result.GetString(1));
            Assert.AreEqual(false, result.Read());
        }

        [TestMethod]
        public void Mock_ReturnsRow_TypedData_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow(new { Id = 11, Name = "Denis" });

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual("Denis", result.GetString(1));
            Assert.AreEqual(false, result.Read());
        }

        [TestMethod]
        public void Mock_ReturnsRow_PrimitiveType_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow(11);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual(false, result.Read());
        }

        [TestMethod]
        public void Mock_ReturnsScalarValue_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarValueFromFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => 14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarValueFromParameter_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => 2 * (c.Parameters.First(p => p.ParameterName == "MyParam").Value as int?));

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            cmd.Parameters.Add(new MockDbParameter() { ParameterName = "MyParam", Value = 7 });
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_SecondMockFound_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("NO"))
                .ReturnsScalar(99);

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsTableFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsTable(c => new MockTable(
                    columns: new[] { "X" },
                    rows: new object[,]
                        {
                            { 10 }
                        }));

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ...";     // This string contains 10 chars
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalarFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .ReturnsScalar(c => c.CommandText.Length);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ...";     // This string contains 10 chars
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(10, result);
        }

    }
}
