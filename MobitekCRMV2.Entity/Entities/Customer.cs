using MobitekCRMV2.Entity.Enums;
using System.Collections.Generic;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// İlişkilendirme notları User Entitysinde mevcut.
    /// </summary>
    public class Customer : EntityBase<string>
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyOfficialWebsite { get; set; }
        public CustomerType CustomerType { get; set; }

        #region NavigationProperties
        public string CustomerRepresentativeId { get; set; }
        public User CustomerRepresentative { get; set; }
        public List<Project> Projects { get; set; }
        #endregion
    }
}
