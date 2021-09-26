# DbMocker - Simple Database Mocker for UnitTests

## Introduction

This .NET library simplifies data mocking for UnitTests, to avoid a connection to a relational database.
DbMocker use the standard Microsoft .NET DbConnection object. So, you can mock any toolkit, 
including EntityFramework, Dapper or ADO.NET; And for all database servers (SQL Server, Oracle, SQLite).

First, add the [DbMocker NuGet packages](https://www.nuget.org/packages/DbMocker).
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

/* Create a text file "123-EMPLOYEES.txt" with this content
   And set the build property to "Embedded resource".
        Id        Name          Age
        (int)     (string)      (int?)

        10        Scott         21
        20        Bill          NULL
*/

[TestMethod]
public void UnitTest0()
{
    var conn = new MockDbConnection();

    // The text file "123-EMPLOYEES.txt" is embedded in this project.
    // See the Samples folder.
    //  - 123       is an identifier (as you want)
    //  - EMPLOYEES is the CommandText Tag 
    //    See https://docs.microsoft.com/en-us/ef/core/querying/tags
    conn.Mocks.LoadTagsFromResources("123-EMPLOYEES");

    // Call your "classic" methods to tests
    var data = GetEmployees(conn);

    // DbMocker read the embedded file 
    // and associated the content to the tag
    Assert.AreEqual("Scott", data[0][1]);
    Assert.AreEqual("Bill", data[1][1]);
}

[TestMethod]
public void UnitTest1()
{
    var conn = new MockDbConnection();

    // When a specific SQL command is detected,
    // Don't execute the query to your database engine (SQL Server, Oracle, SQLite, ...),
    // But returns this _Table_.
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

See [https://apps72.com](https://apps72.com) for more information.







## Conditions

Use the `When` method to describe the condition to be detected. 
This condition is based on a Lambda expression containing a CommandText or Parameters check.

```CSharp
conn.Mocks
    .When(cmd => cmd.CommandText.StartsWith("SELECT") &&
                 cmd.Parameters.Count() == 0)
    .ReturnsTable(...);
```

Use the `WhenTag` method to detect query containing a row starting with `-- MyTag`. 
This is compatible with EFCore 2.2, containing a new extension method `WithTag` to identity a request.

```CSharp
conn.Mocks
    .WhenTag("MyTag")
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

Using a **MockTable.WithColumns()** typed columns. In this case, columns are defined using a tuple (ColumnName, ColumnType).

```CSharp
conn.Mocks
    .WhenAny()
    .ReturnsTable(MockTable.WithColumns(("ID", typeof(int?)),
                                        ("Name", typeof(string)))
                            .AddRow(null, "Scott")
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

using a **CSV string** with all data.
The first row contains the column names.
The first data row defines types for each columns (like in a Excel importation).

```CSharp
string csv = @" Id	Name	Birthdate
                1	Scott	1980-02-03
                2	Bill	1972-01-12
                3	Anders	1965-03-14 ";

conn.Mocks
    .WhenAny()
    .ReturnsTable(MockTable.FromCsv(csv));
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

## Check the SQL Server query syntax

Call the method `Mocks.HasValidSqlServerCommandText()` 
to check if your string **CommandText** respect the SQL Server syntax...
without connection to SQL Server (but using the [Microsoft.SqlServer.SqlParser](https://www.nuget.org/packages/Microsoft.SqlServer.SqlParser) package).

```CSharp
conn.Mocks
    .HasValidSqlServerCommandText()
    .WhenAny()
    .ReturnsScalar(14);
```

So the `CommandText="SELECT ** FROM EMP"` (double *)
will raised a **MockException** with the message "Incorrect syntax near '*'".

You can also define a default value using the `MockDbConnection.HasValidSqlServerCommandText` property.

```CSharp
var conn = new MockDbConnection()
{
    HasValidSqlServerCommandText = true
};
```

## Releases

## Version 1.19
- Use a direct reference to SqlParser.dll.
  Thanks [yyalkovich](https://github.com/yyalkovich).

## Version 1.18
- Add methods `ThrowsException()` to throw an exception when a condition has occured.
  Thanks [wessonben](https://github.com/wessonben).

## Version 1.17
- Fix consistent NewLine detection (\n, \r or both), used by `WhenTag` method.
  Thanks [martinsmith1968](https://github.com/martinsmith1968).

## Version 1.16
- Add `MockTable.FromType<T>` method to fill a mock table using existing .NET objects.
  Thanks [martinsmith1968](https://github.com/martinsmith1968).

## Version 1.15
- Fix `MockDbDataReader.IsDBNull` when the value is null.

## Version 1.14
- Add 'Guid' type in `MockTable.FromFixed` and resource sample files.
- Add a custom message when a sample conversion fails. Ex: `Invalid conversion of "2020-01-32" to "DateTime", for column "Col1"`.

## Version 1.13
- Set output value for DbParameter, using `MockResturns.SetParameterValue` method.
  Thanks [unby](https://github.com/unby).
- Fix `MockDbDataReader.HasRows` to return true when at least a row is existing.
  Thanks [htw8441](https://github.com/htw8441).

### Version 1.12
- Fix the fixed format (resource files) to convert values using Invariant Culture.  

### Version 1.10 and 11
- Minor fixes to deploy nuget.

### Version 1.9
- Add `MockResourceOptions.TagSeparator` (default is '-') to allow to include multiple MockTable resource samples. 
  "01-MyTag.txt" and "02-MyTag.txt" are two resource using the same SQL tag (MyTag).

### Version 1.8
- Add `Mocks.LoadTagsFromResources` to include MockTable samples described in Embedded resource text files. 

### Version 1.7
- Add `ReturnsDataset` methods to simulate multiple tables (Thanks [stop-cran](https://github.com/stop-cran)).
- Update the reference to the Nuget **Microsoft.SqlServer.Management.SqlParser** to validate syntaxes of SQL queries.

### Version 1.6
- Add detailed SQL Query in MockException properties (#6).
- Add a new WhenTag method (#7).
- Add a method to validate the syntax of SQL queries without connection to SQL Server (only for SQL Server syntax) (#8).

### Version 1.5
- Add a `MockColumn` class to manage the column type. See example using "typed columns" above.
- Breaking change: to allow typed MockColumn, the property `MockTable.Columns` is now of type MockColumn[] (previously string[]).

### Version 1.4
- Add `MockTable.FromCsv(string)` method.

### Version 1.3
- Add `ReturnsRow(T)` and `ReturnsRow(Func<MockCommand, T>)` methods.
