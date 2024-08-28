using Microsoft.AspNetCore.Identity;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Veritabanını AspnetUsers tablosuna karşılık gelen model.
    /// İçerisinde property olmamasının sebebi Microsoft.AspNetCore.Identity kütüphanesinin IdentityUser class'ını inherit ediyor olmasından ötürüdür.
    /// IdentityUser class'ı içerisinde temel property'leri ( kolonları ) barındırmaktadır. Bu tabloya ekstra bir kolon eklenmek istendiği zaman
    /// User class'ı içerisine yeni property tanımlaması yapılması gerekir.
    /// 
    /// İlişkilendirme notları 21.09.2022: bir müşterinin birçok projesi olabilir, bir projenin bir müşterisi olabilir
    /// --bir müşterinin birden fazla kullanıcısı olabilir, bir kullanıcının birden fazla müşterisi olabilir 
    /// --burada müşteri temsilcisi rolündeki kullanıcı, proje ile dolaylı yoldan bağlantılı olmuş oluyor.
    /// </summary>
    public class User : IdentityUser, IEntity
    {
        public UserType UserType { get; set; } //Seo, Sem, or Customer
        public string Department { get; set; } //??
        public Status Status { get; set; }


        #region Navigation Properties
        virtual public List<Project> CustomerProjects { get; set; } //Müşteri temsilcisi olduğu projeler
        virtual public List<Project> ExpertProjects { get; set; } //Uzmanı olduğu projeler
        virtual public List<Customer> Customers { get; set; } //Temsilcisi olduğu müşteriler

        virtual public List<Todo> Todo { get; set; }

        virtual public UserInfo UserInfo { get; set; }// editor ve yazar ek bilgileri
        virtual public List<NewsSite> NewsSites { get; set; }

        public static implicit operator User(Task<User> v)
        {
            throw new NotImplementedException();
        }


        #endregion


    }
}
