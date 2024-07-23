using EsNestDSL.Core.Enums;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EsNestDSL.Core.Nest
{
    public class NestSearchBuilder<T, S> where T : class where S : class
    {
        /// <summary>
        /// build search
        /// </summary>
        /// <param name="queryContainer"></param>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        public SearchDescriptor<T> BuildSearchQuery(NestSearchContainer<T, S> queryContainer, S searchEntity)
        {
            SearchDescriptor<T> container = new SearchDescriptor<T>();

            container.Query(q => BuildQueryContainer(queryContainer, searchEntity));

            return container;
        }

        /// <summary>
        /// build count search
        /// </summary>
        /// <param name="queryContainer"></param>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        public CountDescriptor<T> BuildCountQuery(NestSearchContainer<T, S> queryContainer, S searchEntity)
        {
            CountDescriptor<T> container = new CountDescriptor<T>();

            container.Query(q => BuildQueryContainer(queryContainer, searchEntity));

            return container;
        }

        /// <summary>
        /// build querycontainer
        /// </summary>
        /// <param name="queryContainer"></param>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private QueryContainerDescriptor<T> BuildQueryContainer(NestSearchContainer<T, S> queryContainer, S searchEntity)
        {
            QueryContainerDescriptor<T> container = new QueryContainerDescriptor<T>();

            container.Bool(b => BuildBoolContainer(queryContainer, searchEntity));

            return container;
        }


        /// <summary>
        /// build bool query
        /// </summary>
        /// <param name="container"></param>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private BoolQueryDescriptor<T> BuildBoolContainer(NestSearchContainer<T, S> container, S searchEntity)
        {
            var boolContainer = new BoolQueryDescriptor<T>();

            List<QueryContainerDescriptor<T>> mustContainers = new List<QueryContainerDescriptor<T>>();
            List<QueryContainerDescriptor<T>> mustNotContainers = new List<QueryContainerDescriptor<T>>();
            List<QueryContainerDescriptor<T>> shouldContainers = new List<QueryContainerDescriptor<T>>();
            List<QueryContainerDescriptor<T>> FilterContainers = new List<QueryContainerDescriptor<T>>();

            Action<QueryPositionEnum, IEnumerable<QueryContainerDescriptor<T>>> addToContainer = (pos, containers) =>
            {
                switch (pos)
                {
                    case QueryPositionEnum.Must:
                        mustContainers.AddRange(containers);
                        break;
                    case QueryPositionEnum.MustNot:
                        mustNotContainers.AddRange(containers);
                        break;
                    case QueryPositionEnum.Should:
                        shouldContainers.AddRange(containers);
                        break;
                    case QueryPositionEnum.Filter:
                        FilterContainers.AddRange(containers);
                        break;
                }
            };

            //build container's components
            if (container.Components.Any())
            {
                var positionGroup = container.Components.GroupBy(g => g.Position);
                foreach (var posItem in positionGroup)
                {
                    var query = posItem.Select(s => s.BuildQuery<T>(searchEntity));

                    addToContainer(posItem.Key, query);
                }
            }

            //if has nest,then build
            if (container.ChildContainers.Any())
            {
                var positionGroup = container.ChildContainers.GroupBy(g => g.Position);
                foreach (var nestItem in positionGroup)
                {
                    var nestContainers = nestItem.Select(n => BuilderNestContainer(n, searchEntity));
                    addToContainer(nestItem.Key, nestContainers);
                }
            }

            //todo: Put all shoulds in must  temporarily
            if (shouldContainers.Any())
            {
                QueryContainerDescriptor<T> shouldQuery = new QueryContainerDescriptor<T>();
                var shouldBool = new BoolQueryDescriptor<T>();
                shouldBool.Should(shouldContainers.ToArray()).MinimumShouldMatch(MinimumShouldMatch.Fixed(1));
                shouldQuery.Bool(q => shouldBool);
                mustContainers.Add(shouldQuery);
            }

            if (mustContainers.Any()) boolContainer.Must(mustContainers.ToArray());
            if (mustNotContainers.Any()) boolContainer.MustNot(mustNotContainers.ToArray());
            if (FilterContainers.Any()) boolContainer.Filter(FilterContainers.ToArray());

            return boolContainer;
        }

        /// <summary>
        /// build nest container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        private QueryContainerDescriptor<T> BuilderNestContainer(NestSearchContainer<T, S> container, S searchEntity)
        {
            QueryContainerDescriptor<T> nestContainer = new QueryContainerDescriptor<T>();

            NestedQueryDescriptor<T> nestedQuery = new NestedQueryDescriptor<T>().Path(container.NestPath);

            var boolQuery = BuildBoolContainer(container, searchEntity);
            nestedQuery.Query(q => q.Bool(b => boolQuery));

            nestContainer.Nested(n => nestedQuery);

            return nestContainer;
        }
    }
}
