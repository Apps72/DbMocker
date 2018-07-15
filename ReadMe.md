# DbMocker - Simple Database Mocker for UnitTests

## Introduction

This .NET library simplifies data mocking for UnitTests, to avoid a connection to a relational database.
DbMocker use the standard Microsoft .NET DbConnection object. So, you can mock any toolkit, 
including EntityFramework, Dapper or ADO.NET; And for all database servers (SQL Server, Oracle, SQLite).

First, add the DbMocker NuGet packages.
Second, mock you SQL requests using this library like this.

Please, contact me if you want other features or to solve bugs.

```CSharp
// Sample method from your DataService
public int GetNumberOfEmployees(DbConnection connection)
{
    using (var cmd = connection.CreateCommand())
    {
        cmd.CommandText = "SELECT COUNT(*) FROM Employees";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}

[TestMethod]
public void UnitTest1()
{
    var conn = new MockDbConnection();

    // When a specific SQL command is detected,
    // Don't execute the query to your SQL Server,
    // But returns this MockTable.
    conn.Mocks
        .When(cmd => cmd.CommandText.StartsWith("SELECT COUNT(*)") &&
                     cmd.Parameters.Count() == 0)
        .ReturnsTable(MockTable.WithColumns("Count")
                               .AddRow(14));

    // Call your "classic" methods to tests
    int count = GetNumberOfEmployees(conn);

    Assert.AreEqual(14, count);
}
```

## Road map

- DataSets are not yet implemented.