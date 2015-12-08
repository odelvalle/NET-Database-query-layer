# NET-Database query layer
A thin database layer to make more easy queries (.NET C#)

###What is it?###
NET-Database query layer components will be a useful addition to any application or website to create SQL queries to your database in a OO way. It supports the majority of different database types such as SQL Server® MySQL® o PostgreSQL®, but is easy extend to any ADO.NET provider.

###Is this an ORM?###
No, NET-Dabase query layer only allow query to database and not include any persistence model.

###Why use NET-Database query layer if I am using EntityFramework®, NHibernate® or ADO.NET?###
In CQRS architectures, the persistence model (command model) is separated to query model. NET- Database query layer is perfect to implement the query model, allowing it to maintain in a different process or running on different hardware.

More info about CQRS: http://martinfowler.com/bliki/CQRS.html

Command-query separation principle: every method should either be a command that performs an action, or a query that returns data to the caller, but not both. .

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
Un método sobre-cargado de esta factoría permite crear una instancia sin necesidad de incluir un objeto Mapper. Para estos casos solo se podrá usar NET-Database query layer para obtener resultados encapsulados en Datatable, Datareader o Scalar 

```csharp
var queryRunner = QueryRunner.CreateHelper("MsSQL");
```

###Query Model###

Todo el modelo de consulta parte de la interfaz ISqlQuery
```csharp
public interface ISqlQuery
{
    string Expression { get; }
    IDictionary<string, object> Parameters { get; }
}
```

- *Expression*: Es el SQL que se desea ejecutar
- *Parameters*: Un diccionario que contiene los parámetros usados en la consulta

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

**Get first column of the first row from query result**
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

NET-Database query layer se apoya en una rama del proyecto Slapper.AutoMapper https://github.com/randyburden/Slapper.AutoMapper publicada en Github https://github.com/odelvalle/Slapper.AutoMapper. Esta librería permite mediante definiciones de nombres, convertir objetos dinámicos en tipos estáticos. 

Las consultas que obtienen directamente objetos de transferencia de datos, implementan la interfaz ISqlSpecification que a su vez hereda de ISqlQuery
```csharp
public interface ISqlSpecification<out TResult> : ISqlQuery
{
   TResult MapResult(IQueryMappers mapper, dynamic source);
}
```
Esta interfaz incluye un método que permite realizar el mapeo entre el resultado de la consulta y el objeto de salida

**Transform Query result to DTO**

**Ejemplo de DTO de salida**
```csharp
public class SimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

**Return IEnumerable<DTO>**
```csharp
public class QuerySpecification : ISqlSpecification<IEnumerable<SimpleDto>>
{
    public QuerySpecification()
    {
        this.Expression = "select...";
    }

    public IEnumerable<SimpleDto> MapResult(IQueryMappers mapper, dynamic source)
    {
        return mapper.MapDynamicToList<SimpleDto>(source);
    }

    public string Expression { get; private set; }
    public IDictionary<string, object> Parameters { get; private set; }
}
```

**Return simple DTO**
```csharp
public class QuerySingleSpecification : ISqlSpecification<SimpleDto>
{
    public QuerySingleSpecification()
    {
        this.Expression = "select...";
    }

    public SimpleDto MapResult(IQueryMappers mapper, dynamic source)
    {
        return mapper.MapDynamicToSingle<SimpleDto>(source);
    }

    public string Expression { get; private set; }
    public IDictionary<string, object> Parameters { get; private set; }
}
```
El mapeo de consultas a objetos DTO se realiza mediante la interfaz IQueryMapper, permitiendo su adaptación a otros mappers distintos a Slapper.AutoMapper
```csharp
public interface IQueryMappers
{
    IEnumerable<TDestination> MapDynamicToList<TDestination>(List<object> source) where TDestination : class;
    TDestination MapDynamicToSingle<TDestination>(IList<object> source) where TDestination : class;
    TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source) where TDestination : class;
}
```
Esta interfaz permite 3 formas de obtener DTOs

- *MapDynamicToList*: Permite retornar una lista de DTO
- *MapDynamicToSingle*: Permite obtener un único DTO de salida, si la consulta retorna más de una fila o ninguna, este método lanza un error
- *MapDynamicToFirstOrDefault*: Retorna la primera fila de la consulta convertida a un DTO, en caso de no obtener respuesta, retorna NULL

**Paged result**

En el caso del paginado, las consultas implementan la interfaz ISqlPageSpecification
```csharp
public interface ISqlPageSpecification<out T> : ISqlSpecification<T>
{
   string SqlCount { get; }
   int Page { get; }
   int ItemsPerPage { get; }
}
```
Esta interfaz permite incluir 2 Query a la base de datos, la que retorna el total de filas y la que retorna los datos. Las propiedades Page y ItemsPerPage son para su uso dentro del SQL que desea ejecutar. NET-Database query layer no realiza la paginación, tan solo facilita la encapsulación de la lógica.

El resultado de una consulta paginada es retornada mediante un objeto PageSqlResult<T>
```csharp
public class PageSqlResult<T>
{
    public long TotalItems { get; set; }
    public long TotalPages { get; set; }
    public long CurrentPage { get; set; }

    public T Result { get; set; }
}
```

**Execute Query**

```csharp
// Return IEnumerable<SimpleDto>
var resultList = queryRunner.Execute(new QuerySpecification());
```
```csharp
// Return SimpleDto
var singleDto = queryRunner.Execute(new QuerySingleSpecification());
```
```csharp
// Return PageSqlResult<SimpleDto>
var pagedList = queryRunner.Execute(new QueryPageSpecification(page:1, itemsPerPages: 2));
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
