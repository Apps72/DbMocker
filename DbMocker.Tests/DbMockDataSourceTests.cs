using System.Threading.Tasks;
using Apps72.Dev.Data.DbMocker;
using Apps72.Dev.Data.DbMocker.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockDataSourceTests
    {
        [TestMethod]
        public async Task Mock_DataSource_Default_Test()
        {
            await using var dataSource = new MockDbDataSource();
            await using var connection = await dataSource.OpenConnectionAsync() as MockDbConnection;

            connection.Mocks
                        .When(c => c.CommandText.Contains("SELECT"))
                        .ReturnsScalar(14);

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM EMP WHERE ID = @ID";
                var result = cmd.ExecuteScalar();

                Assert.AreEqual(14, result);
            }
        }
    }
}
