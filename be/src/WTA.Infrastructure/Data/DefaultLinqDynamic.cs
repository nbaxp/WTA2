using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Mapster;
using WTA.Application.Abstractions;
using WTA.Application.Application;
using WTA.Application.Extensions;

namespace WTA.Infrastructure.Data;

[Service<ILinqDynamic>]
public class DefaultLinqDynamic : ILinqDynamic
{
    static DefaultLinqDynamic()
    {
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(3);
    }

    public IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args)
    {
        return DynamicQueryableExtensions.OrderBy(source, ordering, args);
    }

    public List<TModel> ToList<TEntity, TModel>(IQueryable<TEntity> source) where TModel : class
    {
        var list = source.ToList().Select(o => o.To<TModel>()).ToList();
        if (typeof(TModel).BaseType != null && typeof(TModel).BaseType!.IsGenericType && typeof(TModel).BaseType!.GetGenericTypeDefinition() == typeof(BaseTreeModel<>))
        {
            var method = typeof(BaseTreeModelExtensions).GetMethod(nameof(BaseTreeModelExtensions.IdentityResolution));
            list = (List<TModel>)method!.MakeGenericMethod(typeof(TModel)).Invoke(null, new object[] { list })!;
            //BaseTreeModelExtensions.IdentityResolution(list as List<BaseTreeModel<TModel>>);
        }
        return list;
    }

    public IQueryable<T> Where<T>(IQueryable<T> source, string queryString, params object[] args)
    {
        return DynamicQueryableExtensions.Where(source, queryString, args);
    }

    public IQueryable<TEntity> Where<TEntity, TModel>(IQueryable<TEntity> source, TModel model) where TModel : class
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
                        source = DynamicQueryableExtensions.Where(source, string.Format(CultureInfo.InvariantCulture, expression, propertyName), propertyValue);
                    }
                }
                var orderByAttributes = attributes.Where(o => o.OperatorType == OperatorType.OrderBy).ToList();
                foreach (var attribute in orderByAttributes.OrderBy(o => o.OperatorType))
                {
                    source = DynamicQueryableExtensions.OrderBy(source, $"{propertyValue}");
                }
            }
        }
        return source;
    }
}
