using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum UserType
    {

        [Display(Name = "Müşteri")]
        Customer,
        [Display(Name = "SEO")]
        SeoExpert,
        [Display(Name = "SEM")]
        SemExpert,
        [Display(Name = "Satış")]
        Sales,
        [Display(Name = "WEB")]
        WebExpert,
        [Display(Name = "Editör")]
        Editor,
        [Display(Name = "Yazar")]
        Writer,
        [Display(Name = "SM")]
        SmExpert

    }
}
