using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

public class UserRole : BaseEntity, IAssociation
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
