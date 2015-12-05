namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;

    using global::ADO.Query.SqlQuery;

    class QuerySimple : ISqlQuery
    {
        public QuerySimple()
        {
            this.Expression = "select id as Id, name as Name from table_in_database";
        }

        public string Expression { get; private set; }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}
