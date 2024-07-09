using System.ComponentModel.DataAnnotations;

namespace EnumsPdfGenerator.Entities.CustomFolder;

public enum CustomEnums
{
    [Display(Name = "Custom-1")]
    First = 1,

    [Display(Name = "Custom-2")]
    Second = 2,
    Third,
    DefaultName = 99,
}
