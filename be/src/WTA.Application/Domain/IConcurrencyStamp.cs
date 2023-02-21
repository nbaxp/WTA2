namespace WTA.Application.Domain;

public interface IConcurrencyStamp
{
    public string? ConcurrencyStamp { get; set; }
}
