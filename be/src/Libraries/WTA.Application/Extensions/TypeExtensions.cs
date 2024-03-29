using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using WTA.Application.Abstractions.Controllers;
using WTA.Application.Application;

namespace WTA.Application.Extensions;

public static class TypeExtensions
{
    public static string GetDisplayName(this Type type)
    {
        var scope = App.Services?.CreateScope();
        var localizer = scope?.ServiceProvider.GetService<IStringLocalizer>();
        var key = type.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.Name;
        return localizer!.GetString(key);
    }

    public static object GetMetadataForType(this Type modelType, IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var meta = scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(modelType);
        return meta.GetSchema(serviceProvider);
    }

    public static object GetSchema(this ModelMetadata meta, IServiceProvider serviceProvider, ModelMetadata? parent = null)
    {
        var modelType = meta.UnderlyingOrModelType;
        var title = meta.ContainerType == null ? modelType.GetDisplayName() : meta.GetDisplayName();
        if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(PaginationModel<,>))
        {
            var genericType = modelType.GetGenericArguments()[0];
            var modelAttribute = genericType.GetCustomAttributes().FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(IModelAttribute))) as IModelAttribute;
            if (modelAttribute != null)
            {
                title = modelAttribute.EntityType.GetDisplayName();
            }
            else
            {
                title = genericType.GetDisplayName();
            }
        }
        var showForDisplay = meta.ShowForDisplay;
        if (meta is DefaultModelMetadata defaultMeta)
        {
            var attribute = defaultMeta.Attributes.PropertyAttributes?.FirstOrDefault(o => o.GetType() == typeof(HiddenInputAttribute));
            if (attribute != null)
            {
                showForDisplay = (attribute as HiddenInputAttribute)!.DisplayValue;
            }
        }
        var schema = new Dictionary<string, object>
        {
          { "title",title },
          { "description", meta.Description! },
          { "format", meta.DataTypeName?.ToLowerCamelCase()! },
          { "template", meta.TemplateHint?.ToLowerCamelCase()! },
          { nameof(meta.ShowForDisplay), showForDisplay },
          { nameof(meta.ShowForEdit), meta.ShowForEdit },
          { nameof(meta.IsReadOnly), meta.IsReadOnly }
        };
        // array
        if (meta.IsEnumerableType)
        {
            if (modelType != meta.ElementMetadata!.ModelType.UnderlyingSystemType)
            {
                schema.Add("type", "array");
                //schema.Add("items", meta.ElementMetadata!.ModelType.GetMetadataForType(serviceProvider));
                schema.Add("items", meta.ElementMetadata.GetSchema(serviceProvider, meta));
            }
        }
        else
        {
            if (!modelType.IsValueType && modelType != typeof(string))
            {
                schema.Add("type", "object");
                schema.Add("$type", modelType.Name);
                var properties = new Dictionary<string, object>();
                foreach (var propertyMetadata in meta.Properties)
                {
                    if (meta.ContainerType != propertyMetadata.ContainerType)
                    {
                        if (propertyMetadata.IsEnumerableType)
                        {
                            //array
                            if (propertyMetadata.ElementType == propertyMetadata.ContainerType)
                            {
                                continue;
                            }
                        }
                        else if (!propertyMetadata.ModelType.IsValueType && propertyMetadata.ModelType != typeof(string))
                        {
                            //object
                            if (propertyMetadata.ModelType == propertyMetadata.ContainerType)
                            {
                                continue;
                            }
                            if (parent != null)
                            {
                                continue;
                            }
                        }
                        properties.Add(propertyMetadata.Name!, propertyMetadata.GetSchema(serviceProvider, meta));
                    }
                }
                schema.Add(nameof(properties), properties);
            }
            else
            {
                schema.Add("type", GetJsonType(modelType));
                schema.Add("$type", modelType.Name.ToLowerCamelCase());
            }
        }
        schema.Add("rules", meta.GetRules(serviceProvider));
        return schema;
    }

    private static string GetJsonType(Type modelType)
    {
        if (modelType == typeof(bool))
        {
            return "boolean";
        }
        else if (modelType == typeof(short) ||
            modelType == typeof(int) ||
            modelType == typeof(long) ||
            modelType == typeof(float) ||
            modelType == typeof(double) ||
            modelType == typeof(decimal))
        {
            return "number";
        }
        return modelType.Name.ToLowerCamelCase();
    }

    public static object GetRules(this ModelMetadata meta, IServiceProvider serviceProvider)
    {
        var pm = (meta as DefaultModelMetadata)!;
        var rules = new List<Dictionary<string, object>>();
        var validationProvider = serviceProvider.GetRequiredService<IValidationAttributeAdapterProvider>();
        var localizer = serviceProvider.GetService<IStringLocalizer>();
        var actionContext = new ActionContext { HttpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext! };
        var provider = new EmptyModelMetadataProvider();
        var modelValidationContextBase = new ModelValidationContextBase(actionContext, meta, new EmptyModelMetadataProvider());
        foreach (var item in pm.Attributes.Attributes)
        {
            if (item is ValidationAttribute attribute && !string.IsNullOrEmpty(attribute.ErrorMessage))
            {
                _ = attribute.ErrorMessage;
                string? message;
                if (attribute is RemoteAttribute)
                {
                    message = localizer![attribute.ErrorMessage!, pm.GetDisplayName()];
                }
                else if (attribute is DataTypeAttribute)
                {
                    if (attribute is FileExtensionsAttribute extensionsAttribute)
                    {
                        message = localizer![attribute.ErrorMessage!, pm.GetDisplayName(), extensionsAttribute.Extensions];
                    }
                    else
                    {
                        message = localizer![attribute.ErrorMessage!, pm.GetDisplayName()];
                    }
                }
                else
                {
                    message = validationProvider.GetAttributeAdapter(attribute!, localizer)?.GetErrorMessage(modelValidationContextBase);
                }
                var rule = new Dictionary<string, object>();
                if (attribute is RegularExpressionAttribute regularExpression)
                {
                    rule.Add("pattern", regularExpression.Pattern);
                }
                else if (attribute is MaxLengthAttribute maxLength)
                {
                    rule.Add("max", maxLength.Length);
                }
                else if (attribute is RequiredAttribute)
                {
                    rule.Add("required", true);
                }
                else if (attribute is CompareAttribute compare)//??
                {
                    rule.Add("validator", "compare");
                    rule.Add("compare", compare.OtherProperty.ToLowerCamelCase());
                }
                else if (attribute is MinLengthAttribute minLength)
                {
                    rule.Add("min", minLength.Length);
                }
                else if (attribute is CreditCardAttribute)
                {
                    rule.Add("validator", "creditcard");
                }
                else if (attribute is StringLengthAttribute stringLength)
                {
                    rule.Add("min", stringLength.MinimumLength);
                    rule.Add("max", stringLength.MaximumLength);
                }
                else if (attribute is RangeAttribute range)
                {
                    rule.Add("type", "number");
                    rule.Add("min", range.Minimum is int minInt ? minInt : (double)range.Minimum);
                    rule.Add("max", range.Maximum is int maxInt ? maxInt : (double)range.Maximum);
                }
                else if (attribute is EmailAddressAttribute)
                {
                    rule.Add("type", "email");
                }
                else if (attribute is PhoneAttribute)
                {
                    rule.Add("validator", "phone");
                }
                else if (attribute is UrlAttribute)
                {
                    rule.Add("type", "url");
                }
                else if (attribute is FileExtensionsAttribute fileExtensions)
                {
                    rule.Add("validator", "accept");
                    rule.Add("extensions", fileExtensions.Extensions);
                }
                else if (attribute is RemoteAttribute remote)
                {
                    rule.Add("validator", "remote");
                    var attributes = new Dictionary<string, string>();
                    remote.AddValidation(new ClientModelValidationContext(actionContext, pm, provider, attributes));
                    rule.Add("remote", attributes["data-val-remote-url"]);
                    //rule.Add("fields", remote.AdditionalFields.Split(',').Where(o => !string.IsNullOrEmpty(o)).Select(o => o.ToLowerCamelCase()).ToList());
                }
                else if (attribute is DataTypeAttribute dataType)
                {
                    var name = dataType.GetDataTypeName();
                    if (name == DataType.DateTime.ToString())
                    {
                        rule.Add("type", "date");
                    }
                }
                else
                {
                    //Console.WriteLine($"{attribute.GetType().Name}");
                }
                rule.Add("message", message!);
                rule.Add("trigger", "change");
                rules.Add(rule);
            }
            else
            {
                //Console.WriteLine($"{item.GetType().Name}");
            }
        }
        return rules;
    }
}
