using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "菜单项", Order = 4)]
[SystemManagement]
public class MenuItem : BaseTreeEntity<MenuItem>
{
    public string? Url { get; set; }
    public string? Icon { get; set; }
}
