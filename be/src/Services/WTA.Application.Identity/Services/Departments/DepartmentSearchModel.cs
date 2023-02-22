using WTA.Application.Abstractions.Controllers;
using WTA.Application.Identity.Domain;

namespace WTA.Application.Identity.Services.Departments;

public class DepartmentSearchModel : PaginationModel<Department>
{
    public Guid? ParentId { get; set; }
}
