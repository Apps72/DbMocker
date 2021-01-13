using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockResourceTests
    {
        [TestMethod]
        public void MockTable_FixedColumns_NormalTypes_Test()
        {
            string content = "Col1    Col2   Col3  \n" +
                             "String  Int32  (int)   ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(3, table.Columns.Length);
            Assert.AreEqual("Col1", table.Columns[0].Name);
            Assert.AreEqual("Col2", table.Columns[1].Name);
            Assert.AreEqual("Col3", table.Columns[2].Name);
            Assert.AreEqual(typeof(string), table.Columns[0].Type);
            Assert.AreEqual(typeof(int), table.Columns[1].Type);
            Assert.AreEqual(typeof(int), table.Columns[2].Type);
        }

        [TestMethod]
        public void MockTable_FixedColumns_MissingType_Test()
        {
            string content = "Col1    Col2 \n" +
                             "(int)";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(2, table.Columns.Length);
            Assert.AreEqual("Col1", table.Columns[0].Name);
            Assert.AreEqual("Col2", table.Columns[1].Name);
            Assert.AreEqual(typeof(int), table.Columns[0].Type);
            Assert.AreEqual(null, table.Columns[1].Type);
        }

        [TestMethod]
        public void MockTable_Int32_Test()
        {
            string content = "Col1       \n" +
                             "(int)      \n" +
                             "123          ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(int), table.Columns[0].Type);
            Assert.AreEqual(123, table.Rows[0, 0]);
        }

        [TestMethod]
        public void MockTable_Decimal_Test()
        {
            string content = "Col1       \n" +
                             "(decimal)  \n" +
                             "123.45       ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(decimal), table.Columns[0].Type);
            Assert.AreEqual((decimal)123.45, table.Rows[0, 0]);
        }

        [TestMethod]
        public void MockTable_TimeSpan_Test()
        {
            string content = "Col1       \n" +
                             "(timespan) \n" +
                             "01:02:03.123     ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(TimeSpan), table.Columns[0].Type);
            Assert.AreEqual(new TimeSpan(0,1,2,3,123), table.Rows[0,0]);
        }

        [TestMethod]
        public void MockTable_DateTime_Test()
        {
            string content = "Col1       \n" +
                             "(date)     \n" +
                             "2020-01-15   ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(DateTime), table.Columns[0].Type);
            Assert.AreEqual(new DateTime(2020, 01, 15), table.Rows[0, 0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void MockTable_InvalidDate_Test()
        {
            string content = "Col1       \n" +
                             "(date)     \n" +
                             "2020-01-32   ";

            var table = MockTable.FromFixed(content);
        }

        [TestMethod]
        public void MockTable_Boolean_Test()
        {
            string content = "Col1       \n" +
                             "(bool)     \n" +
                             "true         ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(Boolean), table.Columns[0].Type);
            Assert.AreEqual(true, table.Rows[0, 0]);
        }

        [TestMethod]
        public void MockTable_String_Test()
        {
            string content = "Col1       \n" +
                             "(string)   \n" +
                             "ABC          ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(String), table.Columns[0].Type);
            Assert.AreEqual("ABC", table.Rows[0, 0]);
        }

        [TestMethod]
        public void MockTable_StringGuillemets_Test()
        {
            string content = "Col1         \n" +
                             "(string)     \n" +
                             "\"ABC DEF\"    ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(String), table.Columns[0].Type);
            Assert.AreEqual("ABC DEF", table.Rows[0, 0]);
        }

        [TestMethod]
        public void MockTable_Guid_Test()
        {
            string content = "Col1                                 \n" +
                             "(guid)                               \n" +
                             "b5acc392-3b9d-4059-9851-3d7344db6e91   ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(typeof(Guid), table.Columns[0].Type);
            Assert.AreEqual(new Guid("b5acc392-3b9d-4059-9851-3d7344db6e91"), table.Rows[0, 0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void MockTable_InvalidGuid_Test()
        {
            string content = "Col1                                 \n" +
                             "(guid)                               \n" +
                             "b5acc392-XXXX-XXXX-XXXX-3d7344db6e91   ";

            var table = MockTable.FromFixed(content);
        }

        [TestMethod]
        public void MockTable_FixedColumns_MissingColumn_Test()
        {
            string content = "Col1           \n" +
                             "int     string";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(1, table.Columns.Length);
            Assert.AreEqual("Col1", table.Columns[0].Name);
            Assert.AreEqual(null, table.Columns[0].Type);
        }

        [TestMethod]
        public void MockTable_FixedColumns_Values_Test()
        {
            string content = "Col1    Col2 \n" +
                             "string  int  \n" +
                             "ABC     0    \n" +
                             "DEF     314  \n" +
                             "GHI     NULL   ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual("ABC", table.Rows[0, 0]);
            Assert.AreEqual(0, table.Rows[0, 1]);

            Assert.AreEqual("DEF", table.Rows[1, 0]);
            Assert.AreEqual(314, table.Rows[1, 1]);

            Assert.AreEqual("GHI", table.Rows[2, 0]);
            Assert.AreEqual(null, table.Rows[2, 1]);
        }

        [TestMethod]
        public void MockTable_FixedColumns_Date_Test()
        {
            string content = "Col1        Col2 \n" +
                             "DateTime    int  \n" +
                             "2020-01-05  99   \n" +
                             "NULL        88     ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(new DateTime(2020, 1, 5), table.Rows[0, 0]);
            Assert.AreEqual(99, table.Rows[0, 1]);

            Assert.AreEqual(null, table.Rows[1, 0]);
            Assert.AreEqual(88, table.Rows[1, 1]);
        }

        [TestMethod]
        public void MockTable_FixedColumns_String_Test()
        {
            string content = "Col1        Col2 \n" +
                             "string      int  \n" +
                             "abc def     99    ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual("abc def", table.Rows[0, 0]);
            Assert.AreEqual(99, table.Rows[0, 1]);
        }

        [TestMethod]
        public void MockTable_FixedColumns_StringGuillemets_Test()
        {
            string content = "Col1        Col2 \n" +
                             "string      int  \n" +
                             "\" abc def \"        ";

            var table = MockTable.FromFixed(content);

            Assert.AreEqual(" abc def ", table.Rows[0, 0]);
            Assert.AreEqual(null, table.Rows[0, 1]);
        }

        [TestMethod]
        public void MockTable_FixedColumns_FromResourceSample1_Test()
        {
            var table = MockTable.FromFixed(Assembly.GetExecutingAssembly(), "DbMocker.Tests.Samples.SampleTable1.txt");

            Assert.AreEqual(4, table.Columns.Length);

            Assert.AreEqual("Id", table.Columns[0].Name);
            Assert.AreEqual("Name", table.Columns[1].Name);
            Assert.AreEqual("Age", table.Columns[2].Name);
            Assert.AreEqual("Male", table.Columns[3].Name);

            Assert.AreEqual(1, table.Rows[0, 0]);
            Assert.AreEqual("Denis", table.Rows[0, 1]);
            Assert.AreEqual(21, table.Rows[0, 2]);
            Assert.AreEqual(true, table.Rows[0, 3]);
        }

        [TestMethod]
        public void MockTable_Tags_FromAssemblyResources_Test()
        {
            var conn = new MockDbConnection();
            conn.Mocks.LoadTagsFromResources(Assembly.GetExecutingAssembly(),
                                             "SampleTable1", "SAMPLETABLE2");

            Assert.AreEqual(2, conn.Mocks.Conditions.Count());

            // SampleTable1
            var cmd1 = conn.CreateCommand();
            cmd1.CommandText = @"-- SampleTable1
                                SELECT ... ";
            var result1 = cmd1.ExecuteScalar();

            Assert.AreEqual(1, result1);

            // SampleTable2
            var cmd2 = conn.CreateCommand();
            cmd2.CommandText = @"-- SAMPLETABLE2
                                SELECT ... ";
            var result2 = cmd2.ExecuteScalar();

            Assert.AreEqual(123, result2);
        }

        [TestMethod]
        public void MockTable_Tags_FromCurrentResources_Test()
        {
            var conn = new MockDbConnection();
            conn.Mocks.LoadTagsFromResources("SampleTable1", "SAMPLETABLE2");

            Assert.AreEqual(2, conn.Mocks.Conditions.Count());
        }

        [TestMethod]
        public void MockTable_Tags_SplitTags_Test()
        {
            var conn = new MockDbConnection();
            conn.Mocks.LoadTagsFromResources("XX-SampleTable1");

            // SampleTable1
            var cmd1 = conn.CreateCommand();
            cmd1.CommandText = @"-- SampleTable1
                                SELECT ... ";
            var result1 = cmd1.ExecuteScalar();

            Assert.AreEqual(10, result1);
        }
    }
}
