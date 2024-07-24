using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Fields;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EsNestDSL.Core.Components
{
    public class AggregationComponent<T> where T : class
    {
        private List<AggregationField<T>> _items;

        public AggregationComponent()
        {
            _items = new List<AggregationField<T>>();
        }

        public AggregationComponent<T> AddAgg(string key, AggregationOperatorEnum operatorEnum,
            Expression<Func<T, object>> field)
            => AddAgg(new AggregationField<T>(key, operatorEnum, field));

        public AggregationComponent<T> AddAgg(AggregationField<T> item)
        {
            _items.Add(item);
            return this;
        }

        public AggregationContainerDescriptor<T> BuildQuery()
        {
            AggregationContainerDescriptor<T> aggsContainer = new AggregationContainerDescriptor<T>();
            if (_items == null || !_items.Any()) return aggsContainer;

            _items.ForEach(item =>
            {
                switch (item.Operator)
                {
                    case AggregationOperatorEnum.Sum:
                        aggsContainer.Sum(item.Key, s => item.BuildSumQuery());
                        break;
                    case AggregationOperatorEnum.Terms:
                        aggsContainer.Terms(item.Key, s => item.BuildTermQuery());
                        break;
                    case AggregationOperatorEnum.CountDistinct:
                        aggsContainer.Cardinality(item.Key, s => item.BuildCountDistinctByQuery());
                        break;
                }
            });

            return aggsContainer;
        }
    }
}
