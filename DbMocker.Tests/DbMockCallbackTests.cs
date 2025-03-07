using Apps72.Dev.Data.DbMocker;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data.Common;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockCallbackTests
    {
        [TestMethod]
        public void Mock_CallsCallback_WhenCallbackSpecified()
        {
            int callsCount = 0;

            MockDbConnection conn = new();

            conn.Mocks
                .WhenAny()
                .Callback(_ => callsCount++)
                .ReturnsScalar(1);

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM EMP";

            int result1 = (int)cmd.ExecuteScalar()!;
            int result2 = (int)cmd.ExecuteScalar()!;
            int result3 = (int)cmd.ExecuteScalar()!;

            Assert.AreEqual(expected: 1, actual: result1);
            Assert.AreEqual(expected: 1, actual: result2);
            Assert.AreEqual(expected: 1, actual: result3);
            Assert.AreEqual(expected: 3, actual: callsCount);
        }
    }
}
