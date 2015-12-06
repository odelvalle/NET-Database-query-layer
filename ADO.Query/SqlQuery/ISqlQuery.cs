namespace ADO.Query.SqlQuery
{
    using System.Collections.Generic;

    public interface ISqlQuery
    {
        string Expression { get; }
        IDictionary<string, object> Parameters { get; }
    }
}
