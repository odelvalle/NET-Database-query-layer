namespace ADO.Query.Helper
{
    using System.Data;
    using ADO.Query.SqlQuery;

    public interface IQueryRunner
    {
        IDbConnection GetConnection();

        TResult Execute<TResult>(ISqlSpecification<TResult> criterial);
        PageSqlResult<TResult> Execute<TResult>(ISqlPageSpecification<TResult> criterial);

        DataTable ExecuteDataTable(ISqlQuery criterial);
        IDataReader ExecuteReader(ISqlQuery criterial);
        T ExecuteScalar<T>(ISqlQuery criterial);
    }
}