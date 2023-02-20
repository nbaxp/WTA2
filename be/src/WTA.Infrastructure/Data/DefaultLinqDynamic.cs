using System.Globalization;
using System.Reflection;
using WTA.Core.Application;
using WTA.Core.Extensions;

namespace WTA.Infrastructure.Data;

public class DefaultLinqDynamic : WTA.Core.Abstractions.ILinqDynamic
{
    public IQueryable<T> Where<T>(IQueryable<T> source, string queryString, params object[] args)
    {
        return source.Where(queryString, args);
    }

    public IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args)
    {
        return source.OrderBy(ordering, args);
    }

    public IQueryable<TEntity> Where<TEntity, TModel>(IQueryable<TEntity> source, TModel model)
    {
        var properties = model!.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyValue = property.GetValue(model, null);
            if (propertyValue != null)
            {
                var attributes = property.GetCustomAttributes<OperatorTypeAttribute>()!;
                var whereAttributes = attributes.Where(o => o.OperatorType != OperatorType.OrderBy).ToList();
                foreach (var attribute in whereAttributes)
                {
                    if (typeof(TEntity).GetProperty(propertyName) == null)
                    {
                        continue;
                    }
                    var expression = attribute.OperatorType.GetAttributeOfType<ExpressionAttribute>()!.Expression;
                    if (attribute.OperatorType != OperatorType.OrderBy)
                    {
                        source = source.Where(string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                    }
                }
                var orderByAttributes = attributes.Where(o => o.OperatorType == OperatorType.OrderBy).ToList();
                foreach (var attribute in orderByAttributes.OrderBy(o => o.OperatorType))
                {
                    source = source.OrderBy($"{propertyValue}");
                }
            }
        }
        return source;
    }
}