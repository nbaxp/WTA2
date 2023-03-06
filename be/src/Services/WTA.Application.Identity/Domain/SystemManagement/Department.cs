using System.ComponentModel.DataAnnotations;
using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain.SystemManagement;

[Display(Name = "部门", Order = 5)]
[SystemManagement]
public class Department : BaseTreeEntity<Department>
{
}
