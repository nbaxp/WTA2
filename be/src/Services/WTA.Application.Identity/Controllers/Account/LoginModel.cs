using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.Identity.Services.Account;

[Description("desc test")]
public class LoginModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display]
    public string Password { get; set; } = null!;

    [Display]
    public bool RememberMe { get; set; }

    [HiddenInput(DisplayValue = false)]
    public string? ReturnUrl { get; set; }

    [StringLength(20, MinimumLength = 5)]
    [Description]
    [Required]
    [Display]
    public string UserName { get; set; } = null!;
}
