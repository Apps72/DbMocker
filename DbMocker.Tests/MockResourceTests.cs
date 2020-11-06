using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbMocker.Tests
{
    [TestClass]
    public class MockResourceTests
    {
        [TestMethod]
        public void MockResource_SplitContent_Returns_Test()
        {
            string content = "ab\ncd\ref\n\rgh\r\nij";
            var resource = new MockResource(content);

            Assert.AreEqual(5, resource.Lines.Count());
            Assert.AreEqual("ab", resource.Lines.ElementAt(0));
            Assert.AreEqual("cd", resource.Lines.ElementAt(1));
            Assert.AreEqual("ef", resource.Lines.ElementAt(2));
            Assert.AreEqual("gh", resource.Lines.ElementAt(3));
            Assert.AreEqual("ij", resource.Lines.ElementAt(4));
        }

        [TestMethod]
        public void MockResource_SplitContent_Comments_Test()
        {
            string content = "ab" + Environment.NewLine +
                             "#cd" + Environment.NewLine +
                             "ef" + Environment.NewLine;
            var resource = new MockResource(content);

            Assert.AreEqual(2, resource.Lines.Count());
            Assert.AreEqual("ab", resource.Lines.ElementAt(0));
            Assert.AreEqual("ef", resource.Lines.ElementAt(1));
        }

        [TestMethod]
        public void MockResource_ColumnPositions_Test()
        {
            string content = "Col1    Col2   Col3  \n" +
                             "Data1   Data2  Data3   ";

            var resource = new MockResource(content);
            var fields = resource.Fields.ToArray();

            Assert.AreEqual(3, fields.Length);
            Assert.AreEqual("Col1", fields[0].Name);
            Assert.AreEqual("Col2", fields[1].Name);
            Assert.AreEqual("Col3", fields[2].Name);
            Assert.AreEqual("Data1", fields[0].DataType);
            Assert.AreEqual("Data2", fields[1].DataType);
            Assert.AreEqual("Data3", fields[2].DataType);
            Assert.AreEqual(0, fields[0].Position);
            Assert.AreEqual(8, fields[1].Position);
            Assert.AreEqual(15, fields[2].Position);
        }

        [TestMethod]
        public void MockResource_MoreColumns_Test()
        {
            string content = "Col1    Col2 \n" +
                             "Data1";

            var resource = new MockResource(content);
            var fields = resource.Fields.ToArray();

            Assert.AreEqual(2, fields.Length);
            Assert.AreEqual("Col1", fields[0].Name);
            Assert.AreEqual("Col2", fields[1].Name);
            Assert.AreEqual("Data1", fields[0].DataType);
            Assert.AreEqual(null, fields[1].DataType);
            Assert.AreEqual(0, fields[0].Position);
            Assert.AreEqual(8, fields[1].Position);
        }

        [TestMethod]
        public void MockResource_MoreDataTypes_Test()
        {
            string content = "Col1           \n" +
                             "Data1    Data2   ";

            var resource = new MockResource(content);
            var fields = resource.Fields.ToArray();

            Assert.AreEqual(1, fields.Length);
            Assert.AreEqual("Col1", fields[0].Name);
            Assert.AreEqual("Data1    Data2", fields[0].DataType);
            Assert.AreEqual(0, fields[0].Position);
        }

        [TestMethod]
        public void MockResource_Rows_Test()
        {
            string content = "Col1    Col2    \n" +
                             "Type1   Type2   \n" +
                             "Value1  Value2  \n" +
                             "Value3  Value4  \n";

            var resource = new MockResource(content);
            var rows = resource.Rows.ToArray();

            Assert.AreEqual("Value1", rows[0][0]);
            Assert.AreEqual("Value2", rows[0][1]);
            Assert.AreEqual("Value3", rows[1][0]);
            Assert.AreEqual("Value4", rows[1][1]);
        }
    }
}
