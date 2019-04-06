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
    public class DbMockTags
    {
        [TestMethod]
        public void Mock_SimpleTag_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                 .WhenTag("MyTag")
                 .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"-- MyTag
                                SELECT ... ";
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        public void Mock_MultipleTags_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                 .WhenTag("MyTag")
                 .ReturnsScalar(99);

            var sql = new StringBuilder();
            sql.AppendLine("-- Tag1");
            sql.AppendLine("-- MyTag");
            sql.AppendLine("-- Tag2");
            sql.AppendLine("SELECT ... ");

            var cmd = conn.CreateCommand();
            cmd.CommandText = sql.ToString();
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(99, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Mock_TagNotFound_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                 .WhenTag("MyTag")
                 .ReturnsScalar(99);

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ... ";
            var result = cmd.ExecuteScalar();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Mock_TagMustStartLine_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                 .WhenTag("MyTag")
                 .ReturnsScalar(99);

            var cmd = conn.CreateCommand();     // First char is a space
            cmd.CommandText = @" -- MyTag
                                SELECT ... ";
            var result = cmd.ExecuteScalar();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Mock_TagMustBeAlone_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                 .WhenTag("MyTag")
                 .ReturnsScalar(99);

            var cmd = conn.CreateCommand();     // There are other chars after MyTag
            cmd.CommandText = @"-- MyTag...
                                SELECT ... ";
            var result = cmd.ExecuteScalar();
        }
    }
}
