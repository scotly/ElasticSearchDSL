using ElasticSearchDSL.Core.Enums;
using System;
using System.Linq.Expressions;

namespace ElasticSearchDSL.Core.Fields
{
    public class RangeField<S> : QueryField<S>
    {
        /// <summary>
        /// type.(date/number)
        /// </summary>
        public RangeTypeEnum Type { get; private set; }
        public RangeOperEnum Operator { get; private set; }
        public RangeOperEnum CompareOperator { get; private set; }

        /// <summary>
        /// compare field
        /// </summary>
        public Expression<Func<S, object>> CompareField { get; private set; }
        public RangeField(RangeTypeEnum rangeTypeEnum, Expression<Func<S, object>> field, RangeOperEnum range, Expression<Func<S, object>> compareField = null, RangeOperEnum compareOperator = RangeOperEnum.Non) : base(field)
        {
            Type = rangeTypeEnum;
            Operator = range;
            CompareField = compareField;
            CompareOperator = compareOperator;
        }
    }
}
