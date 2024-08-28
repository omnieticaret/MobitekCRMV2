using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Veritabanı modellerine karşılık gelen class'ların ( aspnet tabloları hariç) miras alacağı class. Tüm tablolarda ortak olarak kullanacak alanlar
    /// burada tanımlanacak.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityBase<T> : IEntity
    {
        public EntityBase()
        {
            CreatedAt = DateTime.Now;
        }
        /// <summary>
        /// Entity identifier
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }

        /// <summary>
        /// Created At
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
