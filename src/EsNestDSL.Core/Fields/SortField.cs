using ElasticSearchDSL.Core.Enums;
using ElasticSearchDSL.Core.Extentions;
using System;
using System.Linq.Expressions;

namespace ElasticSearchDSL.Core.Fields
{
    public class SortField<T>
    {
        public Expression<Func<T, object>> Field { get; private set; }
        public SortTypeEnum OrderBy { get; private set; }
        public string Name { get; private set; }
        public SortField(Expression<Func<T, object>> expression, SortTypeEnum orderByEnum = SortTypeEnum.Asc)
        {
            Field = expression;
            OrderBy = orderByEnum;
            Name = Field.GetESPropertyName();
        }
    }
}
