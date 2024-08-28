using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum ContractType
    {
        [Display(Name = "Yok")]
        Unavailable,
        [Display(Name = "Var")]
        Available
    }
}
