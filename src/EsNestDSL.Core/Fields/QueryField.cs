using ElasticSearchDSL.Core.Extentions;
using System;
using System.Linq.Expressions;

namespace ElasticSearchDSL.Core.Fields
{
    public class QueryField<S>
    {
        /// <summary>
        /// Field
        /// </summary>
        public Expression<Func<S, object>> Field { get; private set; }

        /// <summary>
        /// Nest path
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        public object Value { get; private set; }

        /// <summary>
        /// fullPath format：path.name
        /// </summary>
        public string FullPath { get; private set; }

        public QueryField(Expression<Func<S, object>> field)
        {
            Field = field;
            Name = Field.GetPropertyName();
        }

        /// <summary>
        /// set nest path
        /// </summary>
        /// <param name="path"></param>
        public void SetNestPath(string path)
        {
            Path = path;
            FullPath = !string.IsNullOrWhiteSpace(Path) ? $"{Path}.{Name}" : Name;
        }

        /// <summary>
        /// set field value
        /// </summary>
        /// <param name="val"></param>
        public void SetVal(object val) => Value = val;

        /// <summary>
        /// init field value from searchEntity
        /// </summary>
        /// <param name="searchEntity"></param>
        public void InitValue(S searchEntity) => Value = Field.GetValue(searchEntity);
    }
}
