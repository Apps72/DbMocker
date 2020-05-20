using Apps72.Dev.Data.DbMocker;
using Apps72.Dev.Data.DbMocker.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockDatasetTests
    {
        [TestMethod]
        public void Mock_Dataset_MultipleTables_Test()
        {
            var conn = new MockDbConnection();

            MockTable table1 = MockTable.WithColumns("Col1", "Col2")
                                        .AddRow(11, 12);

            MockTable table2 = MockTable.WithColumns("Col3", "Col4")
                                        .AddRow("MyString", 3.4);

            conn.Mocks
                .WhenAny()
                .ReturnsDataset(table1, table2);

            // First DataTable
            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

            Assert.IsTrue(result.Read());

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual(12, result.GetInt32(1));

            // Second DataTable
            Assert.IsTrue(result.NextResult());
            Assert.IsTrue(result.Read());

            Assert.AreEqual("MyString", result.GetString(0));
            Assert.AreEqual(3.4, result.GetDouble(1));
        }

        [TestMethod]
        public void Mock_Dataset_NoTable_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsDataset();

            // First DataTable
            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

            Assert.IsFalse(result.Read());
            Assert.AreEqual(null, result.GetValue(0));
        }

        [TestMethod]
        public void Mock_Dataset_Func_Test()
        {
            var conn = new MockDbConnection();

            MockTable table1 = MockTable.WithColumns("Col1", "Col2")
                                       .AddRow(11, 12);

            MockTable table2 = MockTable.WithColumns("Col3", "Col4")
                                        .AddRow("MyString", 3.4);

            conn.Mocks
                .WhenAny()
                .ReturnsDataset(mockCmd =>
                {
                    Assert.AreEqual("SELECT ...", mockCmd.CommandText);
                    return new MockTable[] { table1, table2 };
                });

            // First DataTable
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ...";
            DbDataReader result = cmd.ExecuteReader();

            Assert.IsTrue(result.Read());

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual(12, result.GetInt32(1));

            // Second DataTable
            Assert.IsTrue(result.NextResult());
            Assert.IsTrue(result.Read());

            Assert.AreEqual("MyString", result.GetString(0));
            Assert.AreEqual(3.4, result.GetDouble(1));
        }
    }
}
