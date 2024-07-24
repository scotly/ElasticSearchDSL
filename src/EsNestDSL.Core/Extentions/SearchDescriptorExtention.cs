using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Fields;
using Nest;

namespace EsNestDSL.Core.Extentions
{
    public static class SearchDescriptorExtention
    {
        /// <summary>
        /// add page
        /// </summary>
        /// <param name="searchDescriptor"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static SearchDescriptor<T> AddPage<T>(this SearchDescriptor<T> searchDescriptor, int pageIndex,
            int pageSize) where T : class
        {
            pageSize = pageSize <= 0 ? 10 : pageSize;
            return searchDescriptor.From((pageIndex - 1) * pageSize).Size(pageSize).TrackTotalHits();
        }

        /// <summary>
        /// add sort
        /// </summary>
        /// <param name="searchDescriptor"></param>
        /// <param name="sortFields"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <returns></returns>
        public static SearchDescriptor<T> AddSort<T, S>(this SearchDescriptor<T> searchDescriptor,
            params SortField<S>[] sortFields) where T : class where S : class
        {
            if (sortFields == null) return searchDescriptor;

            return searchDescriptor.Sort(s =>
            {
                foreach (var item in sortFields)
                {
                    if (item.OrderBy == SortTypeEnum.Asc)
                        s.Ascending(item.Name);
                    else
                        s.Descending(item.Name);
                }

                return s;
            });
        }
    }
}
