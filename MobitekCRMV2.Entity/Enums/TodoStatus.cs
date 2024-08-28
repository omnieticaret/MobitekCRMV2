using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    public enum TodoStatus
    {
        [Display(Name = "Yeni")]
        New,
        [Display(Name = "İşlemde")]
        InProgress,
        [Display(Name = "Beklemede")]
        Waiting,
        [Display(Name = "Tamamlandı")]
        Done

    }
}
