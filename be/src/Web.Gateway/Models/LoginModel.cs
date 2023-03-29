using System.ComponentModel.DataAnnotations;

namespace Web.Gateway.Models;

public class LoginModel
{
    [Display(Name = "用户名")]
    [Required(ErrorMessage = "必填项")]
    public string? UserName { get; set; }

    [Display(Name = "密码")]
    [Required(ErrorMessage = "必填项")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}
