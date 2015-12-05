namespace ADO.Query.SqlQuery
{
    public class PageSqlResult<T>
    {
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }
        public long CurrentPage { get; set; }

        public T Result { get; set; }
    }
}
