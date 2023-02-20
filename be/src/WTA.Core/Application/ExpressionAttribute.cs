﻿namespace WTA.Core.Application;

[AttributeUsage(AttributeTargets.Field)]
public class ExpressionAttribute : Attribute
{
    public ExpressionAttribute(string expression)
    {
        this.Expression = expression;
    }

    public string Expression { get; }
}