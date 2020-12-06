using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockDapperDymamicParameters
    {
        [TestMethod]
        public void Mock_Return_OutputParameter_When_Use_Dapper_DynamicParameters()
        {
            const int actualOutValue = 999;
            var conn = new MockDbConnection();

            conn.Mocks
                .When(c => c.Parameters.Any(a => a.ParameterName == "outPar1"))
                .SetValueForDBParameter("outPar1", actualOutValue)
                .ReturnsScalar(1);
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@par1", 123);
            parameters.Add("@outPar1", direction: System.Data.ParameterDirection.Output, dbType: System.Data.DbType.Int32);
            conn.Execute("Exec proc @in1=@par1, @out1=@outPar1", parameters);

            var outValue = parameters.Get<int>(@"@outPar1");

            Assert.AreEqual(outValue, actualOutValue);
        }
    }
}
