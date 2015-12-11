using System.Collections.Generic;

namespace ADO.Query.Test.Query
{
    using ADO.Query.SqlQuery;

    public class QueryUsers : ISqlQuery
    {
        public QueryUsers()
        {
            this.Expression = "select id as Id, name as Name from User";
        }

        public string Expression { get; private set; }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}
