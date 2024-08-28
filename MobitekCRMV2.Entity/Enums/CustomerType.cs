using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum CustomerType
    {
        [Display(Name = "Standart")]
        Standart,
        [Display(Name = "Premium")]
        Premium,
        [Display(Name = "Kobi")]
        Kobi
    }
}
