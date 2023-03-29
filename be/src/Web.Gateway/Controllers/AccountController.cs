using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Gateway.Models;

namespace Web.Gateway.Controllers;

public class AccountController : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginModel model, string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (ModelState.IsValid)
        {
            if (model.UserName == "admin" && model.Password == "aA123456!")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim(ClaimTypes.Role, "Administrator"),
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity)).ConfigureAwait(false);

                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            ModelState.AddModelError("", "用户名或密码错误");
        }
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await HttpContext.SignOutAsync().ConfigureAwait(false);
        return LocalRedirect(returnUrl);
    }
}
