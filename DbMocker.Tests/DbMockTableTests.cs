using Apps72.Dev.Data.DbMocker;
using Apps72.Dev.Data.DbMocker.Helpers;
using DbMocker.Tests.SampleTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTableTests
    {
        [TestMethod]
        public void Mock_ReturnsScalar_MockTable_Test()
        {
            var conn = new MockDbConnection();

            MockTable table = MockTable.Empty()
                                 .AddColumns("Col1")
                                 .AddRow(11);

            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            DbCommand cmd = conn.CreateCommand();
            object result = cmd.ExecuteScalar();

            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void Mock_ReturnsScalar_MockTable_TypedColumns_Test()
        {
            var conn = new MockDbConnection();

            MockTable table = MockTable.Empty()
                                 .AddColumns(("Col1", typeof(int)))
                                 .AddRow(11);

            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            DbCommand cmd = conn.CreateCommand();
            object result = cmd.ExecuteScalar();

            Assert.AreEqual(11, result);
        }

        [TestMethod]
        public void Mock_ReturnsSimple_MockTable_Test()
        {
            var conn = new MockDbConnection();

            MockTable table = MockTable.WithColumns("Col1", "Col2")
                                 .AddRow(11, 12)
                                 .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

            result.Read();

            Assert.AreEqual(11, result.GetInt32(0));
            Assert.AreEqual(12, result.GetInt32(1));

            result.Read();

            Assert.AreEqual(13, result.GetInt32(0));
            Assert.AreEqual(14, result.GetInt32(1));
        }

        [TestMethod]
        public void Mock_ReturnsSimple_MockRow_Test()
        {
            var conn = new MockDbConnection();

            MockTable table = MockTable.WithColumns("Col1", "Col2")
                                       .AddRow(11, 12)
                                       .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsRow(table);

            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

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

            MockTable table = MockTable.WithColumns("Col1", "Col2")
                                 .AddRow(null, 12)
                                 .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

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

            MockTable table = MockTable.WithColumns(("Col1", typeof(int?)),
                                                    ("Col2", typeof(int)))
                                       .AddRow(null, 12)
                                       .AddRow(13, 14);
            conn.Mocks
                .WhenAny()
                .ReturnsTable(table);

            DbCommand cmd = conn.CreateCommand();
            DbDataReader result = cmd.ExecuteReader();

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

            object result = conn.CreateCommand().ExecuteScalar();

            Assert.AreEqual(DBNull.Value, result);
        }

        [TestMethod]
        public void Mock_ReturnsSimple_Null_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsScalar<object>(null);

            object result = conn.CreateCommand().ExecuteScalar();

            Assert.AreEqual(DBNull.Value, result);
        }

        [TestMethod]
        public void Mock_ReturnsRow_DBNull_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenAny()
                .ReturnsRow(new { id = 1, Name = DBNull.Value });

            DbDataReader result = conn.CreateCommand().ExecuteReader();

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

            DbDataReader result = conn.CreateCommand().ExecuteReader();

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

            DbCommand cmd = conn.CreateCommand();
            object result = cmd.ExecuteScalar();

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

            DbCommand cmd = conn.CreateCommand();
            object result = cmd.ExecuteScalar();

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

        [TestMethod]
        public void Mock_MockTable_FromType_Test()
        {
            // Arrange
            var sampleInstance = SimpleModel.RandomInstance();

            // Act
            var table = MockTable.FromType<SimpleModel>();

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(5, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[4].Name);

            Assert.AreEqual(sampleInstance.Id.GetType(), table.Columns[0].Type);
            Assert.AreEqual(sampleInstance.Name.GetType(), table.Columns[1].Type);
            Assert.AreEqual(sampleInstance.Description.GetType(), table.Columns[2].Type);
            Assert.AreEqual(sampleInstance.Value.GetType(), table.Columns[3].Type);
            Assert.AreEqual(sampleInstance.TimestampCreated.GetType(), table.Columns[4].Type);
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_BindingFlags_override_Test()
        {
            // Arrange
            var bindingFlags = MockTable.DefaultFromTypeBindingFlags | BindingFlags.Static;
            var sampleInstance = SimpleModel.RandomInstance();

            // Act
            var table = MockTable.FromType<SimpleModel>(propertyBindingFlags: bindingFlags);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(6, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.RandomValue), table.Columns[5].Name);

            Assert.AreEqual(sampleInstance.Id.GetType(), table.Columns[0].Type);
            Assert.AreEqual(sampleInstance.Name.GetType(), table.Columns[1].Type);
            Assert.AreEqual(sampleInstance.Description.GetType(), table.Columns[2].Type);
            Assert.AreEqual(sampleInstance.Value.GetType(), table.Columns[3].Type);
            Assert.AreEqual(sampleInstance.TimestampCreated.GetType(), table.Columns[4].Type);
            Assert.AreEqual(SimpleModel.RandomValue.GetType(), table.Columns[5].Type);
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_RowData_Test()
        {
            // Arrange
            var sampleInstance = SimpleModel.RandomInstance();

            var rowData = Enumerable.Range(1, 5)
                .Select(x => SimpleModel.RandomInstance())
                .ToArray();

            // Act
            var table = MockTable.FromType<SimpleModel>(rowData);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(5, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[4].Name);

            Assert.AreEqual(sampleInstance.Id.GetType(), table.Columns[0].Type);
            Assert.AreEqual(sampleInstance.Name.GetType(), table.Columns[1].Type);
            Assert.AreEqual(sampleInstance.Description.GetType(), table.Columns[2].Type);
            Assert.AreEqual(sampleInstance.Value.GetType(), table.Columns[3].Type);
            Assert.AreEqual(sampleInstance.TimestampCreated.GetType(), table.Columns[4].Type);

            for (var i = 0; i < rowData.Length; ++i)
            {
                Assert.AreEqual(rowData[i].Id, table.Rows[i, 0]);
                Assert.AreEqual(rowData[i].Name, table.Rows[i, 1]);
                Assert.AreEqual(rowData[i].Description, table.Rows[i, 2]);
                Assert.AreEqual(rowData[i].Value, table.Rows[i, 3]);
                Assert.AreEqual(rowData[i].TimestampCreated, table.Rows[i, 4]);
            }
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_BindingFlags_override_and_RowData_Test()
        {
            // Arrange
            var bindingFlags = MockTable.DefaultFromTypeBindingFlags | BindingFlags.Static;
            var sampleInstance = SimpleModel.RandomInstance();

            var rowData = Enumerable.Range(1, 5)
                .Select(x => SimpleModel.RandomInstance())
                .ToArray();

            // Act
            var table = MockTable.FromType<SimpleModel>(rowData, bindingFlags);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(6, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.RandomValue), table.Columns[5].Name);

            Assert.AreEqual(sampleInstance.Id.GetType(), table.Columns[0].Type);
            Assert.AreEqual(sampleInstance.Name.GetType(), table.Columns[1].Type);
            Assert.AreEqual(sampleInstance.Description.GetType(), table.Columns[2].Type);
            Assert.AreEqual(sampleInstance.Value.GetType(), table.Columns[3].Type);
            Assert.AreEqual(sampleInstance.TimestampCreated.GetType(), table.Columns[4].Type);
            Assert.AreEqual(SimpleModel.RandomValue.GetType(), table.Columns[5].Type);

            for (var i = 0; i < rowData.Length; ++i)
            {
                Assert.AreEqual(rowData[i].Id, table.Rows[i, 0]);
                Assert.AreEqual(rowData[i].Name, table.Rows[i, 1]);
                Assert.AreEqual(rowData[i].Description, table.Rows[i, 2]);
                Assert.AreEqual(rowData[i].Value, table.Rows[i, 3]);
                Assert.AreEqual(rowData[i].TimestampCreated, table.Rows[i, 4]);
            }
        }

        [TestMethod]
        public void Mock_CheckException_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenTag("NotFoundTag")
                .ReturnsScalar(99);

            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM EMP";

            DbParameter myParam = cmd.CreateParameter();
            myParam.ParameterName = "MyParam";
            cmd.Parameters.Add(myParam);

            try
            {
                object result = cmd.ExecuteScalar();
            }
            catch (MockException ex)
            {
                Assert.AreEqual("SELECT * FROM EMP", ex.CommandText);
                Assert.AreEqual("MyParam", ex.Parameters.First().ParameterName);
            }
        }
    }
}
