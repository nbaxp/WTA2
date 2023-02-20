namespace WTA.Core.Domain;

public interface IConcurrencyStamp
{
    public string? ConcurrencyStamp { get; set; }
}
