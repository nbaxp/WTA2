namespace WTA.Core.Application;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class OperatorTypeAttribute : Attribute
{
    public OperatorTypeAttribute(OperatorType operatorType)
    {
        this.OperatorType = operatorType;
    }

    public OperatorType OperatorType { get; }
}