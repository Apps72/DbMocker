using System.Data.Common;

namespace Apps72.Dev.Data.DbMocker.Data
{
    public class MockDbDataSource : DbDataSource
    {
        public override string ConnectionString => MockDbConnection.MOCKER_CONNECTION_STRING;

        protected override MockDbConnection CreateDbConnection()
        {
            return new MockDbConnection();
        }
    }
}
