using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using WTA.Application.Authentication;
using WTA.Application.Extensions;
using WTA.Application.Identity.Controllers;
using WTA.Application.Identity.Domain;
using WTA.Application.Json;
using WTA.Core.Abstractions;
using WTA.Core.Application.Token;

namespace WTA.Application.Identity.Services.Account;

[Authorize]
public class AccountController : BaseController
{
    private readonly IStringLocalizer _localizer;
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IdentityOptions _identityOptions;
    private readonly ITokenService _tokenService;

    public AccountController(IStringLocalizer localizer,
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IOptions<IdentityOptions> identityOptions,
        ITokenService tokenService)
    {
        this._localizer = localizer;
        this._userRepository = userRepository;
        this._passwordHasher = passwordHasher;
        this._identityOptions = identityOptions.Value;
        this._tokenService = tokenService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        var model = new LoginModel { ReturnUrl = returnUrl };
        return this.Result(model);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] LoginModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = this.ValidateUser(model);
                if (result.Status == ValidateUserStatus.Successful)
                {
                    if (this.Request.IsJsonRequest())
                    {
                        var tokenResult = this._tokenService.CreateAuth2TokenResult(model.UserName, model.RememberMe);
                        return Json(tokenResult, new JsonSerializerOptions { PropertyNamingPolicy = new UnderlineJsonNamingPolicy() });
                    }
                    else
                    {
                        var key = UnderlineJsonNamingPolicy.ToUnderline(nameof(OAuth2TokenResult.AccessToken));
                        var accessTokenForCookie = this._tokenService.CreatAccessTokenForCookie(model.UserName, model.RememberMe, out var timeout);
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.Now.Add(timeout)
                        };
                        if (Request.Cookies.Keys.Contains(key))
                        {
                            Response.Cookies.Delete(key);
                        }
                        Response.Cookies.Append(key, accessTokenForCookie);
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction(null);
                        }
                    }
                }
                ModelState.AddModelError("", _localizer[result.Status.ToString()]);
                return this.Result(model);
            }
            return BadRequest(ModelState.ToErrors());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [NonAction]
    public ValidateUserResult ValidateUser(LoginModel model)
    {
        var result = new ValidateUserResult();
        var user = _userRepository.Query().FirstOrDefault(o => o.UserName == model.UserName);
        if (user != null)
        {
            result.User = user;
            if (SupportsUserLockout(user))
            {
                if (user.LockoutEnd.HasValue)
                {
                    if (user.LockoutEnd.Value >= DateTimeOffset.UtcNow)
                    {
                        result.Status = ValidateUserStatus.LockedOut;
                    }
                    else
                    {
                        user.AccessFailedCount = 0;
                        user.LockoutEnd = null;
                        UpdateUser();
                    }
                }
            }
            if (user.PasswordHash == _passwordHasher.HashPassword(model.Password, user.SecurityStamp!))
            {
                result.Status = ValidateUserStatus.Successful;
            }
            else
            {
                result.Status = ValidateUserStatus.WrongPassword;
                if (SupportsUserLockout(user))
                {
                    if (user.AccessFailedCount + 1 < _identityOptions.MaxFailedAccessAttempts)
                    {
                        user.AccessFailedCount += 1;
                    }
                    else
                    {
                        user.LockoutEnd = DateTimeOffset.UtcNow.Add(_identityOptions.DefaultLockoutTimeSpan);
                        user.AccessFailedCount = 0;
                        result.Status = ValidateUserStatus.LockedOut;
                    }
                    UpdateUser();
                }
            }
        }
        else
        {
            result.Status = ValidateUserStatus.NotExist;
        }
        return result;
    }

    private bool SupportsUserLockout(User user)
    {
        return _identityOptions.SupportsUserLockout && user.LockoutEnabled;
    }

    private void UpdateUser()
    {
        this._userRepository.SaveChangesAsync();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        var key = UnderlineJsonNamingPolicy.ToUnderline(nameof(OAuth2TokenResult.AccessToken));
        Response.Cookies.Delete(key);
        return Ok(true);
    }
}
