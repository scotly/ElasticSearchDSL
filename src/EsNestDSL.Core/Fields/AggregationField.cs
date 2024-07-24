using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Extentions;
using Nest;
using System;
using System.Linq.Expressions;

namespace EsNestDSL.Core.Fields
{
    public class AggregationField<T> where T : class
    {
        public string Key { get; protected set; }
        public AggregationOperatorEnum Operator { get; protected set; } = AggregationOperatorEnum.Terms;

        public Expression<Func<T, object>> Field { get; protected set; }


        public AggregationField(string key, AggregationOperatorEnum operatorEnum, Expression<Func<T, object>> field)
        {
            this.Key = key;
            this.Operator = operatorEnum;
            this.Field = field;
        }

        /// <summary>
        /// terms
        /// </summary>
        /// <returns></returns>
        public TermsAggregationDescriptor<T> BuildTermQuery()
        {
            TermsAggregationDescriptor<T> termsAggregate = new TermsAggregationDescriptor<T>();

            termsAggregate.Field(new Field(Field.GetESPropertyName()));

            return termsAggregate;
        }

        /// <summary>
        /// sum
        /// </summary>
        /// <returns></returns>
        public SumAggregationDescriptor<T> BuildSumQuery()
        {
            SumAggregationDescriptor<T> sumAggregate = new SumAggregationDescriptor<T>();
            sumAggregate.Field(new Field(Field.GetESPropertyName()));
            return sumAggregate;
        }

        /// <summary>
        /// count(distinct xxx)
        /// </summary>
        /// <returns></returns>
        public CardinalityAggregationDescriptor<T> BuildCountDistinctByQuery()
        {
            CardinalityAggregationDescriptor<T> cardinalityAggregate = new CardinalityAggregationDescriptor<T>();
            cardinalityAggregate.Field(new Field(Field.GetESPropertyName()));

            return cardinalityAggregate;
        }
    }
}
