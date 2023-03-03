using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "菜单项", Order = 4)]
public class MenuItem : BaseTreeEntity<MenuItem>
{
    public string? Url { get; set; }
    public string? Icon { get; set; }
}
