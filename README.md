# NET-Database query layer
A thin database layer to make more easy queries (.NET C#)

###¿Qué es?###
NET-Database query layer permite de manera fácil y rápida, realizar consultas a bases de datos Microsoft SQL Server® MySQL® o PostgreSQL®, aunque puede ser extendida fácilmente a cualquier base de datos que implemente un controlador para ADO.NET

###¿Es un ORM?###
No, NET-Dabase query layer como su nombre indica, solo permite realizar consultas a base de datos y no incluye ningún mecanismo de persistencia.

###¿Por qué usar NET-Database query layer si ya tengo EntityFramework® o NHibernate®?###
En arquitecturas como CQRS, el modelo de persistencia (command model) va separado del modelo de consulta (query model). En este modelo de consulta es donde encaja perfectamente NET-Database query layer, permitiendo mantener las consultas a datos en un proceso diferente o incluso, ejecutándose en un hardware diferente. 

Para más información sobre CQRS: http://martinfowler.com/bliki/CQRS.html

###¿Configurando NET-Database query layer? usando app.config o web.config ###

**Para Microsoft SQL Server®**
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

**Para MySQL®**
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

**Para PostgreSQL®**
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

###¿Cómo instanciar NET-Database query layer?###
NET-Database query layer cuenta con una factoría que le permite crear la instancia correcta según el tipo de base de datos a la que se desea acceder.

```csharp
var queryRunner = AdoHelper.CreateHelper("MsSQL", new QueryMapper());
```
El primer parámetro usado en la factoría es el alias declarado en la configuración. El segundo parámetro es la instancia de una clase que permite mapear el resultado de consultas directamente con objetos de transferencia de datos (DTO)

En el caso de usar inyección de dependencia, también se debe usar esta factoría (Ejemplo con Unity)

```csharp
var container = new UnityContainer();
container.RegisterType<IAdoHelper>(new InjectionFactory(c => AdoHelper.CreateHelper("MsSQL", new QueryMapper())));

var queryRunner = container.Resolve<IAdoHelper>();
```
Un método sobre-cargado de esta factoría permite crear una instancia sin necesidad de incluir un objeto Mapper. Para estos casos solo se podrá usar NET-Database layer query para obtener resultados encapsulados en Datatable, Datareader o Scalar 

```csharp
var queryRunner = AdoHelper.CreateHelper("MsSQL");
```

###Modelo de consultas###

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

**Creando mi primera Query**
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

**Obteniendo un DataTable**
```csharp
var queryRunner = AdoHelper.CreateHelper("MsSQL", new QueryMapper());
var dt = queryRunner.ExecuteDataTable(new QuerySimple());
```

**Obteniendo un Datareader**
```csharp
var queryRunner = AdoHelper.CreateHelper("MsSQL", new QueryMapper());
var dr = queryRunner.ExecuteReader(new QuerySimple());
```

**Obteniendo la primera columna de la primera fila del resultado de la consulta**
```csharp
var queryRunner = AdoHelper.CreateHelper("MsSQL", new QueryMapper());
var id = queryRunner.ExecuteScalar<int>(new QuerySimple());
```

**Creando consultas con parámetros**
```csharp
class QueryWithParameters : ISqlQuery
{
    public QueryWithParameters(int id, string name)
    {
       this.Expression = "select id as Id, name as Name from table_in_database where id = :id and name = :name";

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

**Obteniendo objetos desde la consulta**

NET-Database query layer se apoya en una rama del proyecto Slapper.AutoMapper https://github.com/randyburden/Slapper.AutoMapper publicada en Github https://github.com/odelvalle/Slapper.AutoMapper. Esta librería permite mediante definiciones de nombres, convertir objetos dinámicos en tipos estáticos. 

Las consultas que obtienen directamente objetos de transferencia de datos, implementan la interfaz ISqlSpecification que a su vez hereda de ISqlQuery
```csharp
public interface ISqlSpecification<out TResult> : ISqlQuery
{
   TResult MapResult(IQueryMappers mapper, dynamic source);
}
```
Esta interfaz incluye un método que permite realizar el mapeo entre el resultado de la consulta y el objeto de salida

**Creando consultas que retornan un objeto**

**Ejemplo de DTO de salida**
```csharp
public class SimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

**Ejemplo de consulta que retorna una lista de DTO**
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

**Ejemplo de consulta que retorna un DTO**
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
- *MapDynamicToSingle*: Permite obtener un único DTO de salida, si la consulta retorna más de una fila o ninguna, este método retorna una excepción
- *MapDynamicToFirstOrDefault*: Retorna la primera fila de la consulta convertida a un DTO, en caso de no obtener respuesta, retorna NULL

**Obteniendo un objeto paginado mediante la consulta**

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

**Ejecutando consultas que retornan DTOs**

```csharp
// Retorna un IEnumerable<SimpleDto>
var resultList = queryRunner.Execute(new QuerySpecification());
```
```csharp
// Retorna un SimpleDto
var singleDto = queryRunner.Execute(new QuerySingleSpecification());
```
```csharp
// Retorna PageSqlResult<SimpleDto>
var pagedList = queryRunner.Execute(new QueryPageSpecification(page:1, itemsPerPages: 2));
```
###Soporte para ejecutar procedimientos almacenados###

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
