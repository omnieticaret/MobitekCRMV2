using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.PlatformsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Dto.Dtos.UsersDtos;
using MobitekCRMV2.Entity.Entities;
using MobitekCRMV2.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobitekCRMV2.Dto.Dtos.ProjectsDto
{
    public class ProjectDetailDto
    {
        public ProjectListDto2 Project { get; set; }
        public List<PlatformsListDto2> Platforms { get; set; }
        public List<UserListDto2> Users { get; set; }
        public List<CustomerListDto2> Customers { get; set; }
        public string KeywordsAsString { get; set; }
        public string StartDate { get; set; }

        public List<KeywordDto11> Keywords { get; set; } 
        public string MeetingDate { get; set; }
        public string ReportDate { get; set; }
        public List<BackLinkDto11> BackLinks { get; set; }
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
        public SmPlatformDto11 PostSmPlatform { get; set; }


        public ProjectDetailDto()
        {
            Project = new ProjectListDto2();
            Users = new List<UserListDto2>();
            Customers = new List<CustomerListDto2>();
            Platforms = new List<PlatformsListDto2>();
            KeywordsAsString = string.Empty;
            Keywords = new List<KeywordDto11>(); 
            KeywordStatistic = new KeywordStatisticDto();
            PostSmPlatform = new SmPlatformDto11();
            BackLinks = new List<BackLinkDto11>();
            CountryCodeList = new List<string>();
        }
    }

    public class BackLinkDto11
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

    public class SmPlatformDto11
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public int PostCount { get; set; }
        public string Note { get; set; }
        public string ProjectId { get; set; }
    }

    public class KeywordDto11
    {
        public string Id { get; set; }
        public string Keyword { get; set; }
        public bool IsStarred { get; set; }
        public List<KeywordValueDto11> KeywordValues { get; set; }
    }

    public class KeywordValueDto11
    {
        public string Id { get; set; }
        public string CountryCode { get; set; }
        public int Position { get; set; }
        public string Date { get; set; }
    }

}
