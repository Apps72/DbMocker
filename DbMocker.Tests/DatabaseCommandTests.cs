using Apps72.Dev.Data;
using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DatabaseCommandTests
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
        public void Mock_ContainsSql_IntegerScalar_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT"))
                .Returns(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("SELECT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteScalar<int>();

                Assert.AreEqual(14, result);
            }
        }

        [TestMethod]
        public void Mock_ContainsSql_And_Parameter_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.CommandText.Contains("SELECT") && 
                           c.Parameters.Any(p => p.ParameterName == "@ID"))
                .Returns(14);

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
                .Returns(14);

            using (var cmd = new DatabaseCommand(conn))
            {
                cmd.CommandText.AppendLine("INSERT ...");
                cmd.AddParameter("@ID", 1);
                var result = cmd.ExecuteNonQuery();

                Assert.AreEqual(14, result);
            }
        }
    }
}
