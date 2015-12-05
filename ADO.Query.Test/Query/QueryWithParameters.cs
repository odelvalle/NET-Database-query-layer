namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;

    using global::ADO.Query.SqlQuery;

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
}
