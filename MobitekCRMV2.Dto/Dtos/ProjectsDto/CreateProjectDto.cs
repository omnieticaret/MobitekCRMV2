using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.ProjectsDto
{
    public class CreateProjectDto
    {
        public string Url { get; set; }
        public string ProjectType { get; set; }
        public string ExpertId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CustomerId { get; set; }
        public string CustomerTypeUserId { get; set; }
        public string ReportMail { get; set; }
        public string Budget { get; set; }
        public int? ContractKeywordCount { get; set; }
        public string Phone { get; set; }
        public string? ContractType { get; set; }
        public string StartDate { get; set; }
        public string ReportDate { get; set; }
        public string? MeetingDate { get; set; }
        public string PacketInfo { get; set; }
        public string ServerStatus { get; set; }
        public string DevelopmentStatus { get; set; }
        public string PlatformId { get; set; }
        public List<string> CountryCodeList { get; set; }
        public string AccessInfo { get; set; }
        public string Note { get; set; }


    }
}
