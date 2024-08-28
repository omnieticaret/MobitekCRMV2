using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum TargetCheckType
    {
        [Display(Name = "Kontrol Edilmedi")]
        UnChecked,
        [Display(Name = "Eşit ")]
        Equal,
        [Display(Name = "Eşit Değil")]
        NotEqual
    }
}
