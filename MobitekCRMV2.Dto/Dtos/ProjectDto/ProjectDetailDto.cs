using MobitekCRMV2.Dto.Dtos.CustomerDto;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Dto.Dtos.UserDto;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.ProjectDto
{
    public class ProjectDetailDto
    {
        public ProjectListDto Project { get; set; }
        public List<PlatformsListDto> Platforms { get; set; }
        public List<UserListDto> Users { get; set; }
        public List<CustomerListDto> Customers { get; set; }
        public string KeywordsAsString { get; set; }
        public string StartDate { get; set; }

        public List<KeywordDto> Keywords { get; set; } 
        public string MeetingDate { get; set; }
        public string ReportDate { get; set; }
        public List<BackLinkDto> BackLinks { get; set; }
        public string DomainId { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnType { get; set; }
        public string IsStarredFilter { get; set; }
        public string CountryCodeFilter { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? H1 { get; set; }
        public List<string> CountryCodeList { get; set; }
        public KeywordStatisticDto KeywordStatistic { get; set; }
        public SmPlatformDto PostSmPlatform { get; set; }


        public ProjectDetailDto()
        {
            Project = new ProjectListDto();
            Users = new List<UserListDto>();
            Customers = new List<CustomerListDto>();
            Platforms = new List<PlatformsListDto>();
            KeywordsAsString = string.Empty;
            Keywords = new List<KeywordDto>(); 
            KeywordStatistic = new KeywordStatisticDto();
            PostSmPlatform = new SmPlatformDto();
            BackLinks = new List<BackLinkDto>();
            CountryCodeList = new List<string>();
        }
    }

    public class BackLinkDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string UrlFrom { get; set; }
        public string LandingPage { get; set; }
        public string Anchor { get; set; }
        public string SelectDate { get; set; }
        public int Da { get; set; }
        public int Pa { get; set; }
    }

    public class KeywordStatisticDto
    {
        public int PositionChanges { get; set; }
        public int AveragePosition { get; set; }
        public int EstimatedTraffic { get; set; }
    }

    public class SmPlatformDto
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
        public string Note { get; set; }
        public string ProjectId { get; set; }
    }

    public class KeywordDto
    {
        public string Id { get; set; }
        public string Keyword { get; set; }
        public bool IsStarred { get; set; }
        public List<KeywordValueDto> KeywordValues { get; set; }
    }

    public class KeywordValueDto
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public int Position { get; set; }
        public string Date { get; set; }
    }

}
