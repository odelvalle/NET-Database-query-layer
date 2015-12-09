# NET-Database query layer
A thin database layer to make more easy queries (.NET C#)

###What is it?###
NET-Database query layer components will be a useful addition to any application or website to create SQL queries to your database in a OO way. It supports the majority of different database types such as SQL Server® MySQL® o PostgreSQL®, but is easy extend to any ADO.NET provider.

###Is this an ORM?###
No, NET-Dabase query layer only allow query to database and not include any persistence model.

###Why use NET-Database query layer if I am using EntityFramework®, NHibernate® or ADO.NET?###
In CQRS architectures, the persistence model (command model) is separated to query model. NET- Database query layer is perfect to implement the query model, allowing it to maintain in a different process or running on different hardware.

More info about CQRS: http://martinfowler.com/bliki/CQRS.html

Other application is in Command-query separation principle: every method should either be a command that performs an action, or a query that returns data to the caller, but not both. .

More info about Command-query separation http://martinfowler.com/bliki/CommandQuerySeparation.html

###Configure NET-Database query layer. App.config or Web.config ###

**Microsoft SQL Server®**
```xml
<configSections>
	<section name="daProvider" type="ADO.Query.Helper.DataAccessSectionHandler, ADO.Query" />
</configSections>

<daProvider>
	<daProvider alias="MsSQL" type="ADO.Query.Helper.MsSql, ADO.Query" connectionStringName="DBConnection" />
</daProvider>

<connectionStrings>
    <add name="DBConnection" connectionString="YOUR_CONNECTION_STRING" />
</connectionStrings> 
```

**MySQL®**
```xml
<configSections>
	<section name="daProvider" type="ADO.Query.Helper.DataAccessSectionHandler, ADO.Query" />
</configSections>

<daProvider>
	<daProvider alias="MySQL" type="ADO.Query.Helper.MySql, ADO.Query" connectionStringName="DBConnection" />
</daProvider>

<connectionStrings>
	<add name="DBConnection" connectionString="YOUR_CONNECTION_STRING" />
</connectionStrings> 
```

**PostgreSQL®**
```xml
<configSections>
	<section name="daProvider" type="ADO.Query.Helper.DataAccessSectionHandler, ADO.Query" />
</configSections>

<daProvider>
	<daProvider alias="PgSQL" type="ADO.Query.Helper.PgSql, ADO.Query" connectionStringName="DBConnection" />
</daProvider>

<connectionStrings>
	<add name="DBConnection" connectionString="YOUR_CONNECTION_STRING" />
</connectionStrings> 
```

###How create instance of QueryRunner?###
NET-Database query layer implement factory pattern to determinate which type of class to create. 

```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL", new QueryMapper());
```
The first paramaeter allow to specificate the alias used in the configuration: this parameter identify the instance to create. The second parameter allow to pass the query mapper object used to convert query resulto to DTO.

Using dependency injection (Unity)

```csharp
var container = new UnityContainer();
container.RegisterType<IQueryRunner>(new InjectionFactory(c => QueryRunner.CreateHelper("MsSQL", new QueryMapper())));

var queryRunner = container.Resolve<IQueryRunner>();
```
An override method of factory can be used without query mapper object parameter, but in this case your query will be used only to return Datatable, datareader or scalar result.

```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL");
```

###Query Model###

All query model implement ISqlQuery Interface
```csharp
public interface ISqlQuery
{
    string Expression { get; }
    IDictionary<string, object> Parameters { get; }
}
```

- *Expression*: SQL string to execute in database
- *Parameters*: Name and value collection with parameters included in SQL string

**My first Query using NET-Database query layer**
```csharp
class QuerySimple : ISqlQuery
{
    public QuerySimple()
    {
        this.Expression = "select id as Id, name as Name from table_in_database";
    }

    public string Expression { get; private set; }

    public IDictionary<string, object> Parameters { get; private set; }
}
```

**Get DataTable**
```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL");
var dt = queryRunner.ExecuteDataTable(new QuerySimple());
```

**Get Datareader**
```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL");
var dr = queryRunner.ExecuteReader(new QuerySimple());
```

**Get the first column of the first row in the query result**
```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL");
var id = queryRunner.ExecuteScalar<int>(new QuerySimple());
```

**Query model with parameters**
```csharp
class QueryWithParameters : ISqlQuery
{
    public QueryWithParameters(int id, string name)
    {
       this.Expression = "select id as Id, name as Name from table_in_database where id = @id and name = @name";

       this.Parameters = new Dictionary<string, object>
       {
          {"id", id},
          {"name", name}
       };
    }

    public string Expression { get; private set; }

    public IDictionary<string, object> Parameters { get; private set; }
 }
```

**Get DTO from query result**

NET-Database query layer use my branch of Slapper.AutoMapper https://github.com/odelvalle/Slapper.AutoMapper on Github. This library can convert dynamic data into static types and populate complex nested child objects. 

**Transform Query result to DTO**

**Example**
```csharp
public class SimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

**Return IEnumerable\<DTO\>**
```csharp
var result = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToList();
```

**Return Single DTO**
```csharp
var singleDto = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToSingle();
```

**Return First or default**
```csharp
var singleDto = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToFirstOrDefault();
```

**Paged result**

To paged result, must implement the interface ISqlPagedQuery
```csharp
public interface ISqlPagedQuery : ISqlQuery
{
   string SqlCount { get; }
   int Page { get; }
   int ItemsPerPage { get; }
}
```
A sample pagination SQL query
```csharp
public class QueryPageSpecification : ISqlPagedQuery
{
    public QueryPageSpecification(int page, int itemsPerPages)
    {
        this.Expression = "select...";
        this.SqlCount = "select count(*)...";

        this.ItemsPerPage = itemsPerPages;
        this.Page = page;
    }

    public string Expression { get; private set; }
    public IDictionary<string, object> Parameters { get; private set; }

    public string SqlCount { get; private set; }

    public int Page { get; private set; }
    public int ItemsPerPage { get; private set; }
}
```
Execute paged query
```csharp
var pagedList = queryRunner.Execute<SimpleDto>(new QueryPageSpecification(page: 1, itemsPerPages: 2));
```
and will return a PageSqlResult<T>
```csharp
public class PageSqlResult<T>
{
    public long TotalItems { get; set; }
    public long TotalPages { get; set; }
    public long CurrentPage { get; set; }

    public IEnumerable<T> Result { get; set; }
}
```

###Store procedure support###

De momento no se soporta la ejecución de procedimientos almacenados

Enjoy ;)

###License###

MIT License:
http://opensource.org/licenses/MIT

Copyright (c) 2015, Omar del Valle ( http://odelvalle.com )
All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
