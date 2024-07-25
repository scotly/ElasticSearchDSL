using ElasticSearchDSL.Core.Enums;
using ElasticSearchDSL.Core.Extentions;
using ElasticSearchDSL.Core.Fields;
using Nest;
using System;
using System.Linq.Expressions;

namespace ElasticSearchDSL.Core.Components
{
    public class RangeComponent<S> : Component<S>
    {
        public RangeComponent(Expression<Func<S, object>> expression, RangeTypeEnum rangeType, RangeOperEnum range,
            Expression<Func<S, object>> compareField = null, RangeOperEnum compareOperator = RangeOperEnum.Non,
            QueryPositionEnum position = QueryPositionEnum.Must, ComponentType componentType = ComponentType.Range)
            : base(new RangeField<S>(rangeType, expression, range, compareField, compareOperator), position, componentType)
        {

        }

        public override QueryContainerDescriptor<T> BuildQuery<T>(S searchEntity)
        {
            if (QueryField.Value == null)
                QueryField.InitValue(searchEntity);

            QueryContainerDescriptor<T> container = new QueryContainerDescriptor<T>();

            RangeField<S> rangeField = QueryField as RangeField<S>;

            string fieldPath = string.Format("{0}{1}", string.IsNullOrWhiteSpace(rangeField.Path) ? "" : rangeField.Path + ".", rangeField.Name);

            if (rangeField.Type == RangeTypeEnum.Date)
            {
                var dateRangeQuery = new DateRangeQueryDescriptor<T>();
                dateRangeQuery.Field(fieldPath);

                if (rangeField.Operator != RangeOperEnum.Non && rangeField.Value != null)
                    GetDataRange(dateRangeQuery, rangeField.Operator, rangeField.Value as string);

                if (rangeField.CompareOperator != RangeOperEnum.Non && rangeField.CompareField != null)
                    GetDataRange(dateRangeQuery, rangeField.CompareOperator, rangeField.CompareField.GetValue(searchEntity) as string);

                container.DateRange(d => dateRangeQuery);
            }
            else if (rangeField.Type == RangeTypeEnum.Num)
            {
                var numRangeQuery = new NumericRangeQueryDescriptor<T>();
                numRangeQuery.Field(fieldPath);

                if (rangeField.Operator != RangeOperEnum.Non && rangeField.Value != null)
                    GetNumRange(numRangeQuery, rangeField.Operator, Convert.ToDouble(rangeField.Value));

                if (rangeField.CompareOperator != RangeOperEnum.Non && rangeField.CompareField != null)
                {
                    GetNumRange(numRangeQuery, rangeField.CompareOperator, Convert.ToDouble(rangeField.CompareField.GetValue(searchEntity)));
                }

                container.Range(d => numRangeQuery);
            }

            return container;
        }

        private void GetDataRange<T>(DateRangeQueryDescriptor<T> container, RangeOperEnum rangeOperator, string val) where T : class
        {
            switch (rangeOperator)
            {
                case RangeOperEnum.gt:
                    container.GreaterThan(DateMath.FromString(val));
                    break;
                case RangeOperEnum.gte:
                    container.GreaterThanOrEquals(DateMath.FromString(val));
                    break;
                case RangeOperEnum.lt:
                    container.LessThan(DateMath.FromString(val));
                    break;
                case RangeOperEnum.lte:
                    container.LessThanOrEquals(DateMath.FromString(val));
                    break;

                default:
                    break;
            }

        }

        private void GetNumRange<T>(NumericRangeQueryDescriptor<T> container, RangeOperEnum rangeOperator, double val) where T : class
        {
            switch (rangeOperator)
            {
                case RangeOperEnum.gt:
                    container.GreaterThan(val);
                    break;
                case RangeOperEnum.gte:
                    container.GreaterThanOrEquals(val);
                    break;
                case RangeOperEnum.lt:
                    container.LessThan(val);
                    break;
                case RangeOperEnum.lte:
                    container.LessThanOrEquals(val);
                    break;

                default:
                    break;
            }

        }
    }
}
