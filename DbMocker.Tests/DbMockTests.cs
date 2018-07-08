using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTests
    {
        //[TestMethod]
        //public void Execute_MockConnection_Test()
        //{
        //    var conn = new Apps72.Dev.Data.DbMocker.MockDbConnection();
        //    //conn.Mock.ForSqlEquals(" SELECT ... ").WithParameter("@ID", 1).Returns(456);
        //    //conn.Mock.ForSqlContains(" X > 2 ").Returns(data, data2, data3);
        //    var data = new object[,] 
        //    {
        //        { "Col1", "Col2", "Col3" },
        //        {  0,      1,      2 }, 
        //        {  9,      8,      7 }
        //    };

        //    var cmd = new DatabaseCommand(conn);
        //    cmd.CommandText.AppendLine(" SELECT GETDATE() ");
        //    var value = cmd.ExecuteScalar<int>();

        //    Assert.AreEqual(456, value);
        //}

        [TestMethod]
        public void Mock_ContainsSql_ReturnsInteger_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .Returns(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ContainsSql_ReturnsScalarInteger_Test()
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
        public void Mock_SecondMockFound_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("NO"))
                .Returns(99);

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .Returns(14);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(14, result);
        }

        [TestMethod]
        public void Mock_ReturnsFunction_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .Returns(c => c.CommandText.Length);

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
