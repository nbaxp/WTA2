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
using WTA.Core.Extensions;

namespace WTA.Application.Extensions;

public static class TypeExtensions
{
    public static string GetDisplayName(this Type type)
    {
        return type.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.Name;
    }

    public static object? GetMetadataForType(this Type modelType, IServiceProvider serviceProvider, bool showForList = false)
    {
        using var scope = serviceProvider.CreateScope();
        var meta = scope.ServiceProvider.GetRequiredService<IModelMetadataProvider>().GetMetadataForType(modelType);
        return meta.GetSchema(serviceProvider, showForList);
    }

    public static object? GetSchema(this ModelMetadata meta, IServiceProvider serviceProvider, bool showForList = false)
    {
        var modelType = meta.UnderlyingOrModelType;

        var schema = new Dictionary<string, object>
    {
      { "title", meta.GetDisplayName() },
      { "description", meta.Description! },
      { "format", meta.DataTypeName?.ToLowerCamelCase()! },
      { "template", meta.TemplateHint?.ToLowerCamelCase()! },
      { nameof(meta.ShowForDisplay), meta.ShowForDisplay },
      { nameof(meta.ShowForEdit), meta.ShowForEdit },
      { nameof(meta.IsReadOnly), meta.IsReadOnly }
    };
        ModelPropertyCollection? metaProperties = null;
        if (meta.IsEnumerableType)
        {
            schema.Add("type", "array");
            metaProperties = meta.ElementMetadata!.Properties;
        }
        else
        {
            if (!modelType.IsValueType && modelType != typeof(string))
            {
                schema.Add("type", "object");
                metaProperties = meta.Properties;
            }
            else
            {
                schema.Add("type", meta.UnderlyingOrModelType.Name.ToLowerCamelCase());
            }
        }
        if (metaProperties != null)
        {
            var properties = new Dictionary<string, object>();
            foreach (var propertyMetadata in metaProperties)
            {
                if (propertyMetadata.IsEnumerableType)
                {
                    if (!showForList)
                    {
                        continue;
                    }
                    else if (meta.MetadataKind == ModelMetadataKind.Property)
                    {
                        continue;
                    }
                }
                var propertyProperties = propertyMetadata.GetSchema(serviceProvider);
                if (propertyProperties != null)
                {
                    properties.Add(propertyMetadata.Name!, propertyProperties);
                }
            }
            schema.Add(nameof(properties), properties);
        }
        schema.Add("rules", meta.GetRules(serviceProvider));
        return schema;
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
                    rule.Add("min", range.Minimum is int ? (int)range.Minimum : (double)range.Minimum);
                    rule.Add("max", range.Maximum is int ? (int)range.Maximum : (double)range.Maximum);
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
