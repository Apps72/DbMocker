using Apps72.Dev.Data;
using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockSqlParserTests
    {
        [TestMethod]
        public void Mock_SqlParser_Valid_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.HasValidSqlServerCommandText())
                .ReturnsScalar(99);
            
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"DECLARE @MyVar AS INT;
                                SELECT * FROM EMP WHERE EMPNO = @MyVar;";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void Mock_SqlParser_Invalid_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.HasValidSqlServerCommandText())
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ** FROM EMP";

            try
            {
                var result = cmd.ExecuteScalar();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }
    }
}
