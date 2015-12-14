using System.Collections.Generic;

namespace ADO.Query.Test.Query
{
    using ADO.Query.SqlQuery;

    public class QueryOneToMany : ISqlQuery
    {
        public QueryOneToMany()
        {
            this.Expression = @"select u.id as Id, u.name as Name, p.id as Phones_Id, p.phone as Phones_Phone 
                                    from [user] u left join [phones] p on (p.usid = u.id)";
        }
        public string Expression { get; private set; }

        public IDictionary<string, object> Parameters { get; private set; }
    }
}
