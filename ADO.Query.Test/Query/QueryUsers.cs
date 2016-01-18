namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;
    using ADO.Query.SqlQuery;

    public class QueryUsers : ISqlQuery
    {
        public QueryUsers()
        {
            this.Expression = "select id as Id, name as Name from [User]";
        }

        public QueryUsers(int id)
        {
            this.Expression = "select id as Id, name as Name from [User] where id = @id";

            this.Parameters = new Dictionary<string, object>
            {
                {"id", id}                                      
            };
        }

        public string Expression { get; private set; }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}
