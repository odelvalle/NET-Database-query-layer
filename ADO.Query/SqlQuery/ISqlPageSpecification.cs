namespace ADO.Query.SqlQuery
{
    public interface ISqlPageSpecification<out T> : ISqlSpecification<T>
    {
        string SqlCount { get; }
        int Page { get; }
        int ItemsPerPage { get; }
    }
}
