using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.BaseData.Controllers;

[Route("base-data/[controller]/[action]")]
[ApiExplorerSettings(GroupName = nameof(BaseData))]
public class BaseController : Controller
{
}
