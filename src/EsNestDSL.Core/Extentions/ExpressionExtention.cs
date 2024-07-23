using Nest;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;

namespace EsNestDSL.Core.Extentions
{
    public static class ExpressionExtention
    {
        public static object GetValue<S>(this Expression<Func<S, object>> expression, S entity)
        {
            var sType = typeof(S);
            // 创建一个表达式树，访问私有成员 _value
            var parameter = Expression.Parameter(sType);

            string propertyName = expression.GetFieldName();

            // 访问私有成员 _value
            var field = Expression.Property(parameter, propertyName);

            var property = sType.GetProperty(propertyName);

            // 编译表达式树，生成一个可以访问私有成员 转成真实类型
            var lambda =
                Expression.Lambda<Func<S, object>>(
                    Expression.Convert(Expression.Property(parameter, property), typeof(object)), parameter);

            // 调用方法
            var func = lambda.Compile();

            return func(entity);
        }


        public static string GetFieldName<S>(this Expression<Func<S, object>> expression) =>
            expression.GetMemberExpression().Member.Name;

        public static string GetPropertyName<S>(this Expression<Func<S, object>> expression)
        {
            var member = expression.GetMemberExpression().Member;
            var jsonPropertyAttr = member.GetCustomAttribute(typeof(JsonPropertyNameAttribute)) as JsonPropertyNameAttribute;

            return jsonPropertyAttr == null
                ? member.Name
                : jsonPropertyAttr.Name;
        }

        public static string GetESPropertyName<S, R>(this Expression<Func<S, R>> expression)
        {
            var member = (expression.Body as MemberExpression ??
                          (expression.Body as UnaryExpression).Operand as MemberExpression).Member;

            var keywordAttr = member.GetCustomAttribute(typeof(KeywordAttribute)) as KeywordAttribute;
            if (keywordAttr != null) return keywordAttr.Name;

            var dateAttr = member.GetCustomAttribute(typeof(DateAttribute)) as DateAttribute;
            if (dateAttr != null) return dateAttr.Name;

            var textAttr = member.GetCustomAttribute(typeof(TextAttribute)) as TextAttribute;
            if (textAttr != null) return textAttr.Name;

            return member.Name;
        }
        private static MemberExpression GetMemberExpression<S>(this Expression<Func<S, object>> expression) => 
            (expression.Body as MemberExpression ??
                          (expression.Body as UnaryExpression).Operand as MemberExpression);
    }
}
