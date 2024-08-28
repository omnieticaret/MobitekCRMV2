using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Enums
{
    // Proje Ekleme sayfası için; durum kısmında belirtilecek olan 'aktif pasif' tanımlaması yapıldı.
    public enum Status
    {
        [Display(Name = "Aktif")]
        Active,
        [Display(Name = "Pasif")]
        Passive,
        [Display(Name = "Donduruldu")]
        Frozen


    }
}
