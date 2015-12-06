namespace ADO.Query.SqlQuery
{
    using ADO.Query.Mapper;

    public interface ISqlSpecification<out TResult> : ISqlQuery
    {
        TResult MapResult(IQueryMappers mapper, dynamic source);
    }
}
