using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum ProjectType
    {
        [Display(Name = "SEO")]
        Seo,
        [Display(Name = "SEM")]
        Sem,
        [Display(Name = "WEB")]
        Web,
        [Display(Name = "None")]
        None,
        [Display(Name = "SM")]
        Sm

    }
}
