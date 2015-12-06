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
	<daProvider alias="MsSQL" type="ADO.Query.Helper.MySql, ADO.Query" connectionStringName="DBConnection" />
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
	<daProvider alias="MsSQL" type="ADO.Query.Helper.PgSql, ADO.Query" connectionStringName="DBConnection" />
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

###Modelo de consultas###


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
