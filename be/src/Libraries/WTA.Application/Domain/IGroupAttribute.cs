namespace WTA.Application.Domain;

public interface IGroupAttribute
{
    int DisplayOrder { get; }
    string Icon { get; }
    string Name { get; }
}
