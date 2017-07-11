using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet.Models
{
    public static class ModelBindExtensions
    {
        public static void BindTo<T1, T2>(this T1 data, T2 model,
            Expression<Func<T1, object>> dataExpression, Expression<Func<T2, object>> modelExpression)
            where T1 : INotifyPropertyChanged
        {
            data.Changed(dataExpression, (d, value) =>
            {
                var property = GetPropertyInfo<T2, object>(model, modelExpression);
                var changedValue = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(model, changedValue);
            });
        }

        public static void Changed<T>(this T data, Expression<Func<T, object>> dataExpression,
            Action<T, object> changed)
            where T : INotifyPropertyChanged
        {
            var property = GetPropertyInfo<T, object>(data, dataExpression);
            Action doChange = () =>
            {
                var value = property.GetValue(data);
                changed(data, value);
            };

            data.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == property.Name)
                    doChange();
            };

            doChange();
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            var bodyType = propertyLambda.Body.GetType();

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                var unaryExpression = propertyLambda.Body as UnaryExpression;
                if(unaryExpression == null)
                {
                    throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
                }

                member = unaryExpression.Operand as MemberExpression;
                if (member == null)
                {
                    throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));
                }
            }
                


            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }
    }
}
