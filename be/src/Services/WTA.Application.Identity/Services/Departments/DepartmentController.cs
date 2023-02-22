using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WTA.Application.Extensions;
using WTA.Application.Identity.Controllers;
using WTA.Application.Identity.Domain;
using WTA.Core.Abstractions;
using WTA.Core.Extensions;

namespace WTA.Application.Identity.Services.Departments;

public class DepartmentController : BaseController
{
    private readonly IRepository<Department> _departmentRepository;

    public DepartmentController(IRepository<Department> departmentRepository)
    {
        this._departmentRepository = departmentRepository;
    }

    [HttpGet]
    public IActionResult Index(DepartmentSearchModel model)
    {
        var query = _departmentRepository.Query().AsNoTrackingWithIdentityResolution();
        if (model.ParentId.HasValue)
        {
            var path = _departmentRepository.Query().AsNoTracking().Where(o => o.Id == model.ParentId.Value).Select(o => o.Path).FirstOrDefault();
            query = query.WhereIf(string.IsNullOrEmpty(path),o => o.Path.StartsWith(path!));
        }
        model.TotalCount = query.Count();
        model.Items = query.Skip(model.PageSize * (model.PageIndex - 1))
            .Take(model.PageSize)
            .ToList()
            .Where(o => o.ParentId == null)
            .ToList();
        return this.Result(model);
    }
}
