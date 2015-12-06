
namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;

    using ADO.Query.Mapper;
    using ADO.Query.SqlQuery;
    using ADO.Query.Test.Query.Dto;

    public class QuerySingleSpecification : ISqlSpecification<SimpleDto>
    {
        public QuerySingleSpecification()
        {
            this.Expression = "select...";
        }

        public SimpleDto MapResult(IQueryMappers mapper, dynamic source)
        {
            return mapper.MapDynamicToSingle<SimpleDto>(source);
        }

        public string Expression { get; private set; }
        public IDictionary<string, object> Parameters { get; private set; }
    }
}
