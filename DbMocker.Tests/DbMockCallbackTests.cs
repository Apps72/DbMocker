using Apps72.Dev.Data.DbMocker;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.Common;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockCallbackTests
    {
        [TestMethod]
        public void Mock_CallsCallback_Single()
        {
            var counter = 0;

            MockDbConnection conn = new();

            conn.Mocks
                .WhenAny()
                .Callback(_ => counter++)
                .ReturnsScalar(1);

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";

            var result1 = cmd.ExecuteScalar();
            var result2 = cmd.ExecuteScalar()!;
            var result3 = cmd.ExecuteScalar()!;

            Assert.AreEqual(1, result1);
            Assert.AreEqual(1, result2);
            Assert.AreEqual(1, result3);

            Assert.AreEqual(3, counter);
        }

        [TestMethod]
        public void Mock_CallsCallback_Multiple()
        {
            var counter1 = 0;
            var counter2 = 0;

            MockDbConnection conn = new();

            conn.Mocks
                .WhenAny()
                .Callback(_ => counter1++)
                .Callback(_ => counter2++)
                .ReturnsScalar(1);

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";

            var result1 = cmd.ExecuteScalar();

            Assert.AreEqual(1, counter1);
            Assert.AreEqual(1, counter2);
        }
    }
}
