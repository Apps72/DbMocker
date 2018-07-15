using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTableTests
    {
        [TestMethod]
        public void Mock_ReturnsScalar_MockTable_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.Empty()
                                 .AddColumns("Col1")
                                 .AddRow(11);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void Mock_ReturnsSimple_MockTable_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.Empty()
                                 .AddColumns("Col1", "Col2")
                                 .AddRow(11, 12)
                                 .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual(12, result.GetInt32(1));

            result.Read();

            Assert.AreEqual(13, result.GetInt32(0));
            Assert.AreEqual(14, result.GetInt32(1));
        }

        [TestMethod]
        public void Mock_ReturnsScalar_MockTableSingle_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.SingleCell("Col1", 11);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalar_MockTableSingleWithoutColumnName_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.SingleCell(11);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteScalar();

            Assert.AreEqual(11, result);
        }
    }
}
