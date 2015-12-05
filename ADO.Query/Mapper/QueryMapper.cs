namespace ADO.Query.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Slapper;

    public class QueryMapper : IQueryMappers, IDisposable
    {
        #region Implementation of IMapper

        public QueryMapper()
        {
            AutoMapper.Cache.ClearInstanceCache();
        }

        public IEnumerable<TDestination> MapDynamicToList<TDestination>(List<object> source) where TDestination : class
        {
            return AutoMapper.MapDynamic<TDestination>(source);
        }

        public TDestination MapDynamicToSingle<TDestination>(IList<object> source) where TDestination : class
        {
            var result = AutoMapper.MapDynamic<TDestination>(source);
            return result.Single();
        }

        public TDestination MapDynamicToFirstOrDefault<TDestination>(IList<object> source) where TDestination : class
        {
            var result = AutoMapper.MapDynamic<TDestination>(source);
            return result.FirstOrDefault();
        }

        #endregion

        public void Dispose()
        {
            AutoMapper.Cache.ClearInstanceCache();
        }
    }
}
