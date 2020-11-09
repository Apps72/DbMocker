using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            Assert.AreEqual(3, table.Columns.Length);

            Assert.AreEqual("Id", table.Columns[0].Name);
            Assert.AreEqual("Name", table.Columns[1].Name);
            Assert.AreEqual("Age", table.Columns[2].Name);

            Assert.AreEqual(1, table.Rows[0, 0]);
            Assert.AreEqual("Denis", table.Rows[0, 1]);
            Assert.AreEqual(21, table.Rows[0, 2]);
        }
    }
}
