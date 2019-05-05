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
        public void Mock_SqlParser_Validate_When_Test()
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
        public void Mock_SqlParser_Validate_WhenAny_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .HasValidSqlServerCommandText()
                .WhenAny()
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"DECLARE @MyVar AS INT;
                                SELECT * FROM EMP WHERE EMPNO = @MyVar;";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void Mock_SqlParser_Validate_WhenTag_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .HasValidSqlServerCommandText()
                .WhenTag("MyTag")
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"-- MyTag
                                SELECT * FROM EMP";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void Mock_SqlParser_Invalid_When_Test()
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
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }

        [TestMethod]
        public void Mock_SqlParser_Invalid_HasNextWhen_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .HasValidSqlServerCommandText()
                .When(c => c.CommandText.Contains("FROM EMP"))
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ** FROM EMP";

            try
            {
                var result = cmd.ExecuteScalar();
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }

        [TestMethod]
        public void Mock_SqlParser_Invalid_WhenTag_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .HasValidSqlServerCommandText()
                .WhenTag("MyTag")
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"-- MyTag
                                SELECT ** FROM EMP";

            try
            {
                var result = cmd.ExecuteScalar();
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }


        [TestMethod]
        public void Mock_SqlParser_Invalid_WhenAny_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .HasValidSqlServerCommandText()
                .WhenAny()
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"-- MyTag
                                SELECT ** FROM EMP";

            try
            {
                var result = cmd.ExecuteScalar();
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }

        [TestMethod]
        public void Mock_SqlParser_TwoMock_OneValidate_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("FROM EMP"))
                .ReturnsScalar(99);

            conn.Mocks
                .When(c => c.CommandText.Contains("FROM DEPT") &&
                           c.HasValidSqlServerCommandText())
                .ReturnsScalar(99);

            // Command NON validated
            var cmd1 = conn.CreateCommand();
            cmd1.CommandText = @"SELECT ** FROM EMP";
            var result1 = cmd1.ExecuteScalar();
            Assert.AreEqual(99, result1);

            // Command validated
            var cmd2 = conn.CreateCommand();
            cmd2.CommandText = @"SELECT ** FROM DEPT";

            try
            {
                var result = cmd2.ExecuteScalar();
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }

        [TestMethod]
        public void Mock_SqlParser_GlobalHas_Test()
        {
            var conn = new MockDbConnection()
            {
                HasValidSqlServerCommandText = true
            };

            conn.Mocks
                .When(c => c.CommandText.Contains("FROM EMP"))
                .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT ** FROM EMP";

            try
            {
                var result = cmd.ExecuteScalar();
                Assert.Fail();
            }
            catch (MockException ex)
            {
                Assert.IsTrue(conn.HasValidSqlServerCommandText);
                Assert.IsInstanceOfType(ex, typeof(MockException));
                Assert.IsTrue(ex.InnerException.Message.Contains("Incorrect syntax near '*'"));
            }
        }
    }
}
