using System;
using Apps72.Dev.Data.DbMocker;
using DbMocker.Tests.SampleTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DbMocker.Tests
{
    [TestClass]
    public class DbMockTableFromTypeTests
    {
        [TestMethod]
        public void Mock_MockTable_FromType_Test()
        {
            // Act
            var table = MockTable.FromType<SimpleModel>();

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(7, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.Checksum), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[5].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampModified), table.Columns[6].Name);

            Assert.AreEqual(typeof(Guid), table.Columns[0].Type);
            Assert.AreEqual(typeof(string), table.Columns[1].Type);
            Assert.AreEqual(typeof(string), table.Columns[2].Type);
            Assert.AreEqual(typeof(int), table.Columns[3].Type);
            Assert.AreEqual(typeof(long?), table.Columns[4].Type);
            Assert.AreEqual(typeof(DateTime), table.Columns[5].Type);
            Assert.AreEqual(typeof(DateTime?), table.Columns[6].Type);
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_BindingFlags_override_Test()
        {
            // Arrange
            var bindingFlags = MockTable.DefaultFromTypeBindingFlags | BindingFlags.Static;

            // Act
            var table = MockTable.FromType<SimpleModel>(propertyBindingFlags: bindingFlags);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(8, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.Checksum), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[5].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampModified), table.Columns[6].Name);

            Assert.AreEqual(typeof(Guid), table.Columns[0].Type);
            Assert.AreEqual(typeof(string), table.Columns[1].Type);
            Assert.AreEqual(typeof(string), table.Columns[2].Type);
            Assert.AreEqual(typeof(int), table.Columns[3].Type);
            Assert.AreEqual(typeof(long?), table.Columns[4].Type);
            Assert.AreEqual(typeof(DateTime), table.Columns[5].Type);
            Assert.AreEqual(typeof(DateTime?), table.Columns[6].Type);
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_RowData_Test()
        {
            // Arrange
            var rowData = Enumerable.Range(1, 5)
                .Select(x =>
                {
                    var instance = SimpleModel.RandomInstance();
                    if (x % 2 == 0) instance.CalculateChecksum();
                    if (x % 3 == 0) instance.SetUpdated();
                    return instance;
                })
                .ToArray();

            // Act
            var table = MockTable.FromType<SimpleModel>(rowData);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(7, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.Checksum), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[5].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampModified), table.Columns[6].Name);

            Assert.AreEqual(typeof(Guid), table.Columns[0].Type);
            Assert.AreEqual(typeof(string), table.Columns[1].Type);
            Assert.AreEqual(typeof(string), table.Columns[2].Type);
            Assert.AreEqual(typeof(int), table.Columns[3].Type);
            Assert.AreEqual(typeof(long?), table.Columns[4].Type);
            Assert.AreEqual(typeof(DateTime), table.Columns[5].Type);
            Assert.AreEqual(typeof(DateTime?), table.Columns[6].Type);

            for (var i = 0; i < rowData.Length; ++i)
            {
                Assert.AreEqual(rowData[i].Id, table.Rows[i, 0]);
                Assert.AreEqual(rowData[i].Name, table.Rows[i, 1]);
                Assert.AreEqual(rowData[i].Description, table.Rows[i, 2]);
                Assert.AreEqual(rowData[i].Value, table.Rows[i, 3]);
                Assert.AreEqual(rowData[i].Checksum, table.Rows[i, 4]);
                Assert.AreEqual(rowData[i].TimestampCreated, table.Rows[i, 5]);
                Assert.AreEqual(rowData[i].TimestampModified, table.Rows[i, 6]);
            }
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_BindingFlags_override_and_RowData_Test()
        {
            // Arrange
            var bindingFlags = MockTable.DefaultFromTypeBindingFlags | BindingFlags.Static;

            var rowData = Enumerable.Range(1, 5)
                .Select(x =>
                {
                    var instance = SimpleModel.RandomInstance();
                    if (x % 2 == 0) instance.CalculateChecksum();
                    if (x % 3 == 0) instance.SetUpdated();
                    return instance;
                })
                .ToArray();

            // Act
            var table = MockTable.FromType<SimpleModel>(rowData, bindingFlags);

            // Assert
            Assert.IsNotNull(table);

            Assert.AreEqual(8, table.Columns.Length);

            Assert.AreEqual(nameof(SimpleModel.Id), table.Columns[0].Name);
            Assert.AreEqual(nameof(SimpleModel.Name), table.Columns[1].Name);
            Assert.AreEqual(nameof(SimpleModel.Description), table.Columns[2].Name);
            Assert.AreEqual(nameof(SimpleModel.Value), table.Columns[3].Name);
            Assert.AreEqual(nameof(SimpleModel.Checksum), table.Columns[4].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampCreated), table.Columns[5].Name);
            Assert.AreEqual(nameof(SimpleModel.TimestampModified), table.Columns[6].Name);
            Assert.AreEqual(nameof(SimpleModel.RandomValue), table.Columns[7].Name);

            Assert.AreEqual(typeof(Guid), table.Columns[0].Type);
            Assert.AreEqual(typeof(string), table.Columns[1].Type);
            Assert.AreEqual(typeof(string), table.Columns[2].Type);
            Assert.AreEqual(typeof(int), table.Columns[3].Type);
            Assert.AreEqual(typeof(long?), table.Columns[4].Type);
            Assert.AreEqual(typeof(DateTime), table.Columns[5].Type);
            Assert.AreEqual(typeof(DateTime?), table.Columns[6].Type);
            Assert.AreEqual(typeof(long), table.Columns[7].Type);

            for (var i = 0; i < rowData.Length; ++i)
            {
                Assert.AreEqual(rowData[i].Id, table.Rows[i, 0]);
                Assert.AreEqual(rowData[i].Name, table.Rows[i, 1]);
                Assert.AreEqual(rowData[i].Description, table.Rows[i, 2]);
                Assert.AreEqual(rowData[i].Value, table.Rows[i, 3]);
                Assert.AreEqual(rowData[i].Checksum, table.Rows[i, 4]);
                Assert.AreEqual(rowData[i].TimestampCreated, table.Rows[i, 5]);
                Assert.AreEqual(rowData[i].TimestampModified, table.Rows[i, 6]);
            }
        }

        [TestMethod]
        public void Mock_MockTable_FromType_with_AnonymousType_Test()
        {
            var table = MockTable.FromType(new[]
            {
                new
                {
                    Id = 123,
                    Name = "My Name"
                }
            });

            Assert.AreEqual(123, table.Rows[0, 0]);
            Assert.AreEqual("My Name", table.Rows[0, 1]);
        }

        [TestMethod]
        public void Should_Only_Return_Properties_From_Whitelist()
        {
            using var context = new MyDbContext();

            var entityTypeModel = context.Model.FindEntityTypes(typeof(NavigationModel));
            var properties = entityTypeModel.FirstOrDefault().GetProperties().ToList();
            var table = MockTable.FromType<NavigationModel>(
                null,
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                properties.Select(p => p.Name).ToArray()
            );

            Assert.IsNotNull(table);

            Assert.AreEqual(7, table.Columns.Length);
            Assert.IsFalse(table.Columns.Any(c => c.Name == nameof(NavigationModel.RelatedModels)));
        }
    }

    public class MyDbContext : DbContext
    {
        public DbSet<NavigationModel> SimpleModels { get; set; }
        public DbSet<RelatedModel> RelatedModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("YourConnectionStringHere");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NavigationModel>()
                .HasMany(sm => sm.RelatedModels)
                .WithOne(rm => rm.NavigationModel)
                .HasForeignKey(rm => rm.NavigationModelId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
