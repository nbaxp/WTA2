using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[Display(Name = "部门")]
public class Department : BaseTreeEntity<Department>
{
}
