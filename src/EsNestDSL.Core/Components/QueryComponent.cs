using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Fields;
using Nest;
using System.Collections.Generic;

namespace EsNestDSL.Core.Components
{
    /// <summary>
    /// comm query component
    /// </summary>
    /// <typeparam name="S"></typeparam>
    public class QueryComponent<S> : Component<S>
    {
        public QueryComponent(QueryField<S> field, QueryPositionEnum position, ComponentType componentType = ComponentType.Match) : base(field, position, componentType)
        {
        }

        public override QueryContainerDescriptor<T> BuildQuery<T>(S searchEntity)
        {
            var request = new QueryContainerDescriptor<T>();

            if (QueryField.Value == null)
                QueryField.InitValue(searchEntity);

            switch (ComponentType)
            {
                case ComponentType.Match:
                    request.Match(m => m.Field(QueryField.FullPath).Query(QueryField.Value as string));
                    break;
                case ComponentType.Terms:
                    request.Terms(m => m.Field(QueryField.FullPath).Terms(QueryField.Value as IEnumerable<string>));
                    break;
                case ComponentType.Like:
                    request.MatchPhrase(m => m.Field(QueryField.FullPath).Query(QueryField.Value as string));
                    break;
                case ComponentType.Exists:
                    request.Exists(m => m.Field(QueryField.FullPath));
                    break;
                default:
                    break;
            }
            return request;
        }
    }
}
