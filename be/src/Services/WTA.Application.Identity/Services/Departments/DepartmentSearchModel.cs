using WTA.Application.Abstractions.Controllers;
using WTA.Application.Identity.Domain;

namespace WTA.Application.Identity.Services.Departments;

public class DepartmentSearchModel
{
    public Guid? ParentId { get; set; }
}
