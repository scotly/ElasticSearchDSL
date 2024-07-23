using EsNestDSL.Core.Components;
using EsNestDSL.Core.Enums;
using EsNestDSL.Core.Fields;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EsNestDSL.Core.Nest
{
    public class NestSearchContainer<T, S> where T : class where S : class
    {
        /// <summary>
        /// current container's component list 
        /// </summary>
        public List<Component<S>> Components { get; private set; }

        /// <summary>
        /// childs
        /// </summary>
        public List<NestSearchContainer<T, S>> ChildContainers { get; private set; }

        /// <summary>
        /// nest path
        /// </summary>
        public string NestPath { get; private set; }
        public QueryPositionEnum Position { get; private set; }

        public NestSearchContainer(string nestPath = "", QueryPositionEnum position = QueryPositionEnum.Must)
        {
            this.NestPath = nestPath;
            this.Position = position;
            Components = new List<Component<S>>();
            ChildContainers = new List<NestSearchContainer<T, S>>();
        }

        /// <summary>
        /// add comm query field
        /// </summary>
        /// <param name="field">field expression</param>
        /// <param name="position">position,default:must</param>
        /// <param name="componentType">componentType,default:match</param>
        /// <param name="val">field value;deault:null;not set usually.auto set value from S when component build</param>        
        /// <returns></returns>
        public NestSearchContainer<T, S> AddQuery(Expression<Func<S, object>> field,
            QueryPositionEnum position = QueryPositionEnum.Must,
            ComponentType componentType = ComponentType.Match,
            object val = null)
        {
            var queryField = new QueryField<S>(field);
            queryField.SetVal(val);

            var component = new QueryComponent<S>(queryField, position, componentType);

            AddComponent(component);

            return this;
        }

        public NestSearchContainer<T, S> AddQueryIF(bool condition, Expression<Func<S, object>> field,
            QueryPositionEnum position = QueryPositionEnum.Must,
            ComponentType componentType = ComponentType.Match,
            object val = null)
        {
            if (!condition) return this;

            return AddQuery(field, position, componentType, val);
        }


        /// <summary>
        /// add child Container,use nest usually
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public NestSearchContainer<T, S> AddContainer(NestSearchContainer<T, S> container)
        {
            if (string.IsNullOrWhiteSpace(container.NestPath)) throw new Exception("nest index has must path");

            ChildContainers.Add(container);

            return this;
        }

        #region private methods

        /// <summary>
        /// add component
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="Exception"></exception>
        private void AddComponent(Component<S> component)
        {
            if (Components.Exists(e => e.QueryField.Name == component.QueryField.Name))
                throw new Exception("sample field existed");

            component.QueryField.SetNestPath(NestPath);
            Components.Add(component);
        }

        #endregion
    }
}
