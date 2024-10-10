using MobitekCRMV2.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Entity.Entities
{
    /// <summary>
    /// Bu classın ilişkilendirme notları User Entitysinde mevcut.
    /// </summary>
    public class Project : EntityBase<string>
    {

        public ProjectType ProjectType { get; set; } 

        public string Url { get; set; }
        public string AuthoritativeUrl { get; set; }
        public string Name { get; set; } 
        public string ReportMail { get; set; } 
        public string Phone { get; set; } 
        public string Budget { get; set; } 
        public int ContractKeywordCount { get; set; } 
        public ContractType Contract { get; set; } 

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } 

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; } 

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MeetingDate { get; set; } 

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StatusUpdateDate { get; set; } 
        public Status Status { get; set; } 
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
