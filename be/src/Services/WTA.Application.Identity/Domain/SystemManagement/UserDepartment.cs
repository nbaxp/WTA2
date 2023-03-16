using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

public class UserDepartment : BaseEntity, IAssociation
{
    public Guid UserId { get; set; }
    public Guid DepartmentId { get; set; }
    public User User { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
