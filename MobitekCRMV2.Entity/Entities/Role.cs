using Microsoft.AspNet.Identity.EntityFramework;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Veritabanını AspnetRoles tablosuna karşılık gelen model.
    /// İçerisinde property olmamasının sebebi Microsoft.AspNetCore.Identity kütüphanesinin IdentityRole class'ını inherit ediyor olmasından ötürüdür.
    /// IdentityRole class'ı içerisinde temel property'leri ( kolonları ) barındırmaktadır. Bu tabloya ekstra bir kolon eklenmek istendiği zaman
    /// Role class'ı içerisine yeni property tanımlaması yapılması gerekir.
    /// @Note: bu tabloya kolon ekleme çıkarma gibi işlemler çok nadir gerçekleşir. Bu projede ihtiyaç olacağını sanmıyorum.
    /// </summary>
    public class Role : IdentityRole, IEntity
    {
        public RoleType RoleType { get; set; }
        public string Description { get; set; }
    }
}
