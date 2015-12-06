
namespace ADO.Query.Test.Query
{
    using System.Collections.Generic;

    using ADO.Query.Mapper;
    using ADO.Query.SqlQuery;
    using ADO.Query.Test.Query.Dto;

    public class QueryPageSpecification : ISqlPageSpecification<IEnumerable<SimpleDto>>
    {
        public QueryPageSpecification(int page, int itemsPerPages)
        {
            this.Expression = "select...";
            this.SqlCount = "select count(id)...";

            this.ItemsPerPage = itemsPerPages;
            this.Page = page;
        }

        public IEnumerable<SimpleDto> MapResult(IQueryMappers mapper, dynamic source)
        {
            return mapper.MapDynamicToList<SimpleDto>(source);
        }

        public string Expression { get; private set; }
        public IDictionary<string, object> Parameters { get; private set; }

        public string SqlCount { get; private set; }

        public int Page { get; private set; }

        public int ItemsPerPage { get; private set; }
    }
}
