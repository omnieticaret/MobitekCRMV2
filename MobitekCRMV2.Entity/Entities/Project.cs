using MobitekCRMV2.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Bu classın ilişkilendirme notları User Entitysinde mevcut.
    /// </summary>
    public class Project : EntityBase<string>
    {

        public ProjectType ProjectType { get; set; } //Proje Tipi

        public string Url { get; set; } //Proje Url
        public string AuthoritativeUrl { get; set; } //Proje Url
        public string Name { get; set; } //Proje Adı
        public string ReportMail { get; set; } //Rapor CC
        public string Phone { get; set; } //Telefon
        public string Budget { get; set; } //Bütçe
        public int ContractKeywordCount { get; set; } //Sözleşme kelime sayısı
        public ContractType Contract { get; set; } //Sözleşme

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } //Başlangıç Tarihi

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; } //Rapor Tarihi

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MeetingDate { get; set; } //Toplantı Tarihi

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StatusUpdateDate { get; set; } //statüsünün değiştirilme tarihi
        public Status Status { get; set; } //Durumu
        public string Note { get; set; }
        public string CountryCode { get; set; }
        public string AccessInfo { get; set; }
        public string DevelopmentStatus { get; set; }
        public string PacketInfo { get; set; }
        public string ServerStatus { get; set; }
        public string ExportPosition { get; set; }

        #region Navigation Properties
        public string CustomerId { get; set; }
        virtual public Customer Customer { get; set; }
        public string ExpertId { get; set; }
        virtual public User Expert { get; set; }
        public string CustomerTypeUserId { get; set; }
        virtual public User CustomerTypeUser { get; set; }
        public string PlatformId { get; set; }
        virtual public Platform Platform { get; set; }
        virtual public List<Keyword> Keywords { get; set; }
        virtual public List<SmPlatform> SmPlatforms { get; set; }
        virtual public List<Promotion> Promotions { get; set; }
        public string DomainId { get; set; }
        virtual public Domain Domain { get; set; }

        virtual public List<Todo> Todo { get; set; }

        virtual public List<ContentBudget> ContentBudgets { get; set; }
        #endregion


    }

}
