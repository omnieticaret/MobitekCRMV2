using MobitekCRMV2.Dto.Dtos.ProjectDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobitekCRMV2.Dto.Dtos
{
    public class ProjectListDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Budget { get; set; }
        public string ProjectType { get; set; }

        public string Contracts { get; set; }
        public string ExpertName { get; set; }
        public string ExpertId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReportDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StatusUpdateDate { get; set; }
        public string ElapsedTime { get; set; }

        public string AuthoritativeUrl { get; set; }
        public string Name { get; set; }
        public string ReportMail { get; set; }
        public string Phone { get; set; }
        public int ContractKeywordCount { get; set; }
        public string Contract { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime MeetingDate { get; set; }
        public string Status { get; set; }

        
        public string Note { get; set; }
        public string CountryCode { get; set; }
        public string AccessInfo { get; set; }
        public string DevelopmentStatus { get; set; }
        public string PacketInfo { get; set; }
        public string ServerStatus { get; set; }
        public string ExportPosition { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTypeUserId { get; set; }
        public string PlatformId { get; set; }
        public string DomainId { get; set; }
    }
   
}
