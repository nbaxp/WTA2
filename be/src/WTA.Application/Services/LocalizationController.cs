using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WTA.Application.Services;

[Route("[controller]/[action]")]
public class LocalizationController : Controller
{
    private readonly IStringLocalizer _localizer;
    private readonly LinkParser _linkParser;
    private readonly RequestLocalizationOptions _options;

    public LocalizationController(IOptions<RequestLocalizationOptions> options, IStringLocalizer localizer, LinkParser linkParser)
    {
        this._options = options.Value;
        this._localizer = localizer;
        this._linkParser = linkParser;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(model: $"""
      当前区域:{Thread.CurrentThread.CurrentCulture.Name}[{Thread.CurrentThread.CurrentCulture.NativeName}],
      localizer["test"]：{this._localizer["test"]}
      """);
    }

    [HttpGet]
    public IActionResult List()
    {
        var options = this._options.SupportedUICultures?
                .Select(o => new { Value = o.Name, Label = o.NativeName })
                .ToList();
        return Json(new
        {
            current = CultureInfo.CurrentCulture.Name,
            options,
        });
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string? returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }
}
