namespace WTA.Application.Domain;

public interface IGroup
{
    string Name { get; }
    int DisplayOrder { get; }
    string Icon { get; }
}
