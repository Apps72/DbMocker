using Apps72.Dev.Data.DbMocker;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Common;
using System.Linq;

namespace DbMocker.Tests
{
    [TestClass]
    public class Samples_EFTests
    {
        // Sample method from your DataService
        public int GetNumberOfEmployees(CompanyContext context)
        {
            return context.Employees
                          .TagWith("EMPLOYEES_COUNT")       // This feature is new in EF Core 2.2.
                          .Count();
        }

        // Sample method from your DataService
        public Employee[] GetEmployees(CompanyContext context)
        {
            return context.Employees
                          .TagWith("ALL_EMPLOYEES")         // This feature is new in EF Core 2.2.
                          .ToArray();
        }

        [TestMethod]
        public void EF_NumberOfEmployees_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenTag("EMPLOYEES_COUNT")
                .ReturnsScalar(14);

            using (var context = new CompanyContext(conn))
            {
                int count = GetNumberOfEmployees(context);

                Assert.AreEqual(14, count);
            }
        }

        [TestMethod]
        public void EF_AllEmployees_Test()
        {
            var conn = new MockDbConnection();

            conn.Mocks
                .WhenTag("ALL_EMPLOYEES")
                .ReturnsTable(new MockTable().AddColumns("Id", "Name")
                                             .AddRow(1, "Scott")
                                             .AddRow(2, "Bill"));

            using (var context = new CompanyContext(conn))
            {
                var employees = GetEmployees(context);

                Assert.AreEqual(1, employees[0].Id);
                Assert.AreEqual("Scott", employees[0].Name);
                Assert.AreEqual(2, employees[1].Id);
                Assert.AreEqual("Bill", employees[1].Name);
            }
        }
    }

    #region Entity Framework Context

    public class CompanyContext : DbContext
    {
        private readonly DbConnection _dbConnection;

        public CompanyContext(DbConnection connection)
        {
            _dbConnection = connection;
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_dbConnection);
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    #endregion
}
