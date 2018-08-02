# DbMocker - Simple Database Mocker for UnitTests

## Introduction

This .NET library simplifies data mocking for UnitTests, to avoid a connection to a relational database.
DbMocker use the standard Microsoft .NET DbConnection object. So, you can mock any toolkit, 
including EntityFramework, Dapper or ADO.NET; And for all database servers (SQL Server, Oracle, SQLite).

First, add the (DbMocker NuGet packages)[https://www.nuget.org/packages/DbMocker].
Next, instanciate a `MockDbConnection` and mock you SQL requests using a condition and return a DataTable.

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
        .When(cmd => cmd.CommandText.StartsWith("SELECT") &&
                     cmd.Parameters.Count() == 0)
        .ReturnsTable(MockTable.WithColumns("Count")
                               .AddRow(14));

    // Call your "classic" methods to tests
    int count = GetNumberOfEmployees(conn);

    Assert.AreEqual(14, count);
}
```

Tip: To easily detect SQL queries to intercept, we advise you to add a comment that identifies the SQL query 
and use it in DBMocker.

```CSharp
    cmd.CommandText = " -- [Request to Update Employees] ...";
    ...
    conn.Mocks
        .When(cmd => cmd.CommandText.Contains("[Request to Update Employees]")
        .ReturnsScalar(14);
```

## Conditions

Use the 'When' method to describe the condition to be detected. 
This condition is based on a Lambda expression containing a CommandText or Parameters check.

```CSharp
conn.Mocks
    .When(cmd => cmd.CommandText.StartsWith("SELECT") &&
                 cmd.Parameters.Count() == 0)
    .ReturnsTable(...);
```

Use `WhenAny` to detect all SQL queries. In this case, all queries to the database will return the data specified by WhenAny.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(...);
```

## ReturnsTable

When the previous condition occured, a mocked table will be return:

Creating an new instance of **MockTable**.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(new MockTable().AddColumns("ID", "Name")
                                 .AddRow(1, "Scott")
                                 .AddRow(2, "Bill"));
```

Using a **MockTable.Empty()** table... to complete.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(MockTable.Empty()
                           .AddColumns("ID", "Name")
                           .AddRow(1, "Scott")
                           .AddRow(2, "Bill"));
```

Using a **MockTable.WithColumns()** table... to complete.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(MockTable.WithColumns("ID", "Name")
                           .AddRow(1, "Scott")
                           .AddRow(2, "Bill"));
```

Returning a **MockTable.SingleCell()** table... to complete.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(MockTable.SingleCell("Count", 14));
```

Using an expression to customize the return.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(cmd => cmd.Parameters.Count() > 0 ? 14 : 99);
```

## ReturnsRow

When a condition occured, a single data row will be return.
The specified typed object will generate a MockTable where property names will be the column names
and proerty values will be the first row data.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsRow(new { Id = 1, Name = "Denis" });
```


Using an expression to customize the return.

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsRow(cmd => new { Id = 1, Name = "Denis" });
```

## ReturnsScalar

When a condition occured, a scalar value will be return:

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsScalar<int>(14);
```

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsScalar<int>(cmd => DateTime.Today.Year > 2000 ? 14 : 0);
```

## Releases

### Version 1.3

- Add `ReturnsRow(T)` and `ReturnsRow(Func<MockCommand, T>)` methods.

## Road map

- DataSets are not yet implemented.