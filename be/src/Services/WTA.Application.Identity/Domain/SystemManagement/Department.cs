using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Data;
using WTA.Application.Domain;
using WTA.Application.Identity.Data;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "部门", Order = 5)]
[SystemManagement]
[DbContext<IdentityDbContext>]
public class Department : BaseTreeEntity<Department>
{
    public List<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
}
