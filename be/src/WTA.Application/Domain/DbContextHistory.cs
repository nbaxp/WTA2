namespace WTA.Application.Domain;

public class DbContextHistory : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Hash { get; set; } = null!;
}
