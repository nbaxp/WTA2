using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Json;
using WTA.Application.Abstractions.Token;
using WTA.Application.Extensions;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Controllers.Account;

[Authorize]
public class AccountController : BaseController
{
    private readonly IdentityOptions _identityOptions;
    private readonly ILogger<AccountController> _logger;
    private readonly IStringLocalizer _localizer;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Permission> _permissionRepository;

    public AccountController(ILogger<AccountController> logger,
        IStringLocalizer localizer,
        IPasswordHasher passwordHasher,
        IOptions<IdentityOptions> identityOptions,
        ITokenService tokenService,
        IRepository<User> userRepository,
        IRepository<Permission> permissionRepository)
    {
        this._logger = logger;
        this._localizer = localizer;
        this._passwordHasher = passwordHasher;
        this._identityOptions = identityOptions.Value;
        this._tokenService = tokenService;
        this._userRepository = userRepository;
        this._permissionRepository = permissionRepository;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    #region Login/Logout

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
                    var key = nameof(OAuth2TokenResult.AccessToken).ToUnderline();
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
                    if (this.Request.IsJsonRequest())
                    {
                        var tokenResult = this._tokenService.CreateAuth2TokenResult(model.UserName, model.RememberMe);
                        return Json(tokenResult, new JsonSerializerOptions { PropertyNamingPolicy = new UnderlineJsonNamingPolicy() });
                    }
                    else
                    {
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
            }
            return BadRequest(ModelState.ToErrors());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        var key = nameof(OAuth2TokenResult.AccessToken).ToUnderline();
        Response.Cookies.Delete(key);
        return Ok(true);
    }

    #endregion Login/Logout

    [HttpPost]
    public IActionResult GetUserInfo()
    {
        try
        {
            var user = this._userRepository
                .AsNoTracking()
                .Include(o => o.UserRoles)
                .ThenInclude(o => o.Role)
                .ThenInclude(o => o.RolePermissions)
                .ThenInclude(o => o.Permission)
                .Where(o => o.UserName == User.Identity!.Name);
            return Ok(user);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return Problem(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult GetMenus()
    {
        try
        {
            var permissions = this._permissionRepository
                .AsNoTracking()
                .ToList()
                .Select(o => o.ParentId == null);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            return Problem(ex.Message);
        }
    }

    private ValidateUserResult ValidateUser(LoginModel model)
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
}
