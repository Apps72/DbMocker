using Apps72.Dev.Data.DbMocker;
using Apps72.Dev.Data.DbMocker.Helpers;
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

            var table = MockTable.WithColumns("Col1", "Col2")
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
        public void Mock_ReturnsSimple_MockTableWithNull_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.WithColumns("Col1", "Col2")
                                 .AddRow(null, 12)
                                 .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(null, result.GetValue(0));
            Assert.IsTrue(result.GetFieldType(0) == typeof(object));

            result.Read();

            Assert.AreEqual(13, result.GetValue(0));
            Assert.IsTrue(result.GetFieldType(0) == typeof(object));
        }

        [TestMethod]
        public void Mock_ReturnsSimple_MockTypedTable_Test()
        {
            var conn = new MockDbConnection();

            var table = MockTable.WithColumns(("Col1", typeof(int?)), 
                                              ("Col2", typeof(int)))
                                 .AddRow(null, 12)
                                 .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            var cmd = conn.CreateCommand();
            var result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(null, result.GetValue(0));
            Assert.IsTrue(result.GetFieldType(0) == typeof(int?));

            Assert.AreEqual(12, result.GetValue(1));
            Assert.IsTrue(result.GetFieldType(1) == typeof(int));
        }

        [TestMethod]
        public void Mock_ReturnsSimple_DBNull_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsScalar<object>(DBNull.Value);

            var result = conn.CreateCommand().ExecuteScalar();

            Assert.AreEqual(DBNull.Value, result);
        }

        [TestMethod]
        public void Mock_ReturnsSimple_Null_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsScalar<object>(null);

            var result = conn.CreateCommand().ExecuteScalar();

            Assert.AreEqual(DBNull.Value, result);
        }

        [TestMethod]
        public void Mock_ReturnsRow_DBNull_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow(new { id = 1, Name = DBNull.Value } );

            var result = conn.CreateCommand().ExecuteReader();

            result.Read();

            Assert.AreEqual(1, result.GetInt32(0));
            Assert.AreEqual(true, result.IsDBNull(1));
        }

        [TestMethod]
        public void Mock_ReturnsRow_Null_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow(new { id = 1, Name = (string)null });

            var result = conn.CreateCommand().ExecuteReader();

            result.Read();

            Assert.AreEqual(1, result.GetInt32(0));
            Assert.AreEqual(true, result.IsDBNull(1));
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

        [TestMethod]
        public void Mock_MockTable_FromCsv_Test()
        {
            var conn = new MockDbConnection();

            string csv = @" Id	Name	Birthdate
                            1	Scott	1980-02-03
                            2	Bill	1972-01-12
                            3	Anders	1965-03-14 ";

            var table = MockTable.FromCsv(csv);

            Assert.AreEqual("Id", table.Columns[0].Name);
            Assert.AreEqual("Name", table.Columns[1].Name);

            Assert.AreEqual(3, table.Rows.RowsCount());

            Assert.AreEqual("Scott", table.Rows[0, 1]);
            Assert.IsInstanceOfType(table.Rows[0, 1], typeof(string));

            Assert.AreEqual(3, table.Rows[2, 0]);
            Assert.IsInstanceOfType(table.Rows[2, 0], typeof(int));

            Assert.AreEqual(new DateTime(1972, 1, 12), table.Rows[1, 2]);
            Assert.IsInstanceOfType(table.Rows[1, 2], typeof(DateTime));
        }
        
    }
}
