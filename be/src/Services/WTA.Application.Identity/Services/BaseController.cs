using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.Identity.Controllers;

[Route("identity/[controller]/[action]")]
[ApiExplorerSettings(GroupName = nameof(Identity))]
public class BaseController : Controller
{
}
