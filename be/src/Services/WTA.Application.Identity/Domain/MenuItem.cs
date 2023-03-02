using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "菜单项")]
public class MenuItem : BaseTreeEntity<MenuItem>
{
}
