using WTA.Application.Abstractions.Data;
using WTA.Application.Domain;
using WTA.Application.Identity.Data;

namespace WTA.Application.Identity.Domain.SystemManagement;

[DbContext<IdentityDbContext>]
public class UserDepartment : BaseEntity, IAssociation
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
