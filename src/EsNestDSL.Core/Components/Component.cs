using ElasticSearchDSL.Core.Enums;
using ElasticSearchDSL.Core.Fields;
using Nest;

namespace ElasticSearchDSL.Core.Components
{
    public abstract class Component<S>
    {
        /// <summary>
        /// field postion; must,mustnot,should ...
        /// </summary>
        public QueryPositionEnum Position { get; protected set; }
        public QueryField<S> QueryField { get; protected set; }

        /// <summary>
        /// match term terms ...
        /// </summary>
        public ComponentType ComponentType { get; protected set; }
        public Component(QueryField<S> field, QueryPositionEnum position, ComponentType componentType = ComponentType.Match)
        {
            QueryField = field;
            Position = position;
            ComponentType = componentType;
        }

        /// <summary>
        /// Build QueryContainer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchEntity"></param>
        /// <returns></returns>
        public abstract QueryContainerDescriptor<T> BuildQuery<T>(S searchEntity) where T : class;
    }
}
