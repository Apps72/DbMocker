using Apps72.Dev.Data.DbMocker;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbMocker.Tests
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void Mock_EmptyTable_Test()
        {
            var mockTable = MockTableExtensions.GetEmptyMockTable<RowDto>();

            Assert.AreEqual(0, mockTable.Rows.Length);
            Assert.AreEqual(3, mockTable.Columns.Length);
            Assert.IsFalse(mockTable.Columns.Any(a => a.Name.Equals(nameof(RowDto.Field))));
        }

        [TestMethod]
        public void Mock_ObjectConversion_Test()
        {
            var dto = new RowDto() { Id=1 , Name = "Mock", Price=null};
            var mockTable = dto.ToMockTable();
            Assert.AreEqual(1, mockTable.Rows.GetLength(0));
            Assert.AreEqual(3, mockTable.Columns.Length);
        }

        [TestMethod]
        public void Mock_CollectionConversion_Test()
        {
            var collection = new[] {
                new RowDto() { Id = 1, Name = "Mock1", Price = null },
                new RowDto() { Id = 2, Name = "Mock2", Price = 23 },
                new RowDto() { Id = 3, Name = "Mock3", Price = 1 }};

            var mockTable = collection.ToMockTableFromCollection();
            Assert.AreEqual(3, mockTable.Rows.GetLength(0));
            Assert.AreEqual(3, mockTable.Columns.Length);

            Assert.AreEqual(collection[0].Id, mockTable.Rows[0,0]);
            Assert.AreEqual(collection[1].Name, mockTable.Rows[1,1]);
            Assert.AreEqual(collection[2].Price, mockTable.Rows[2, 2]);
        }

        public class RowDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Field;
            public decimal? Price { get; set; }
            
        }
    }
}
