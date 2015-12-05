namespace ADO.Query.SqlQuery
{
    using System.Collections.Generic;
    using System.Data;

    public interface ISqlQuery
    {
        string Expression { get; }
        IDictionary<string, object> Parameters { get; }
    }
}
