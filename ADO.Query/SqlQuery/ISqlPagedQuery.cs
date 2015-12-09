namespace ADO.Query.SqlQuery
{
    public interface ISqlPagedQuery : ISqlQuery
    {
        string SqlCount { get; }
        int Page { get; }
        int ItemsPerPage { get; }
    }
}
