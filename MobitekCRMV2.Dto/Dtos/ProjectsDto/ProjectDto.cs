using MobitekCRMV2.Dto.Dtos.ContentsBudgetDto;
using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.DomainDto;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Dto.Dtos.PlatformDto;
using MobitekCRMV2.Dto.Dtos.PromotionsDto;
using MobitekCRMV2.Dto.Dtos.SmPlatformsDto;
using MobitekCRMV2.Dto.Dtos.UserDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.ProjectsDto
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public ProjectType ProjectType { get; set; }
        public string Url { get; set; }
        public string AuthoritativeUrl { get; set; }
        public string Name { get; set; }
        public string ReportMail { get; set; }
        public string Phone { get; set; }
        public string Budget { get; set; }
        public int ContractKeywordCount { get; set; }
        public ContractType Contract { get; set; }
        public string StartDate { get; set; }
        public string ReportDate { get; set; }
        public string MeetingDate { get; set; }
        public string StatusUpdateDate { get; set; }
        public Status Status { get; set; }
        public string Note { get; set; }
        public string CountryCode { get; set; }
        public string AccessInfo { get; set; }
        public string DevelopmentStatus { get; set; }
        public string PacketInfo { get; set; }
        public string ServerStatus { get; set; }
        public string ExportPosition { get; set; }

        public string CustomerId { get; set; }
        public CustomerSummaryDto Customer { get; set; }

        public string ExpertId { get; set; }
        public UserSummaryDto Expert { get; set; }

        public string CustomerTypeUserId { get; set; }
        public UserSummaryDto CustomerTypeUser { get; set; }

        public string PlatformId { get; set; }
        public PlatformSummaryDto Platform { get; set; }

        public List<KeywordSummaryDto> Keywords { get; set; }
        public List<SmPlatformSummaryDto> SmPlatforms { get; set; }
        public List<PromotionSummaryDto> Promotions { get; set; }

        public string DomainId { get; set; }
        public DomainSummaryDto Domain { get; set; }

        public List<ContentBudgetSummaryDto> ContentBudgets { get; set; }

        public int CompletedKeywordsCount { get; set; }
        public int TotalKeywordsCount { get; set; }
        public decimal CompletionRate { get; set; }
    }

    public class ProjectListDto
    {
        public List<ProjectSummaryDto> Projects { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<Status, int> StatusDistribution { get; set; }
        public Dictionary<ProjectType, int> TypeDistribution { get; set; }
    }

    public class ProjectSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public ProjectType ProjectType { get; set; }
        public Status Status { get; set; }
        public string ReportDate { get; set; }
        public string CustomerName { get; set; }
        public string ExpertName { get; set; }
        public int KeywordsCount { get; set; }
        public decimal CompletionRate { get; set; }
    }

    public class ProjectCreateUpdateDto
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
        public DateTime StartDate { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime MeetingDate { get; set; }
        public Status Status { get; set; }
        public string Note { get; set; }
        public string CountryCode { get; set; }
        public string AccessInfo { get; set; }
        public string DevelopmentStatus { get; set; }
        public string PacketInfo { get; set; }
        public string ServerStatus { get; set; }
        public string ExportPosition { get; set; }
        public string CustomerId { get; set; }
        public string ExpertId { get; set; }
        public string CustomerTypeUserId { get; set; }
        public string PlatformId { get; set; }
        public string DomainId { get; set; }
    }

    public class ProjectFilterDto
    {
        public string SearchTerm { get; set; }
        public ProjectType? ProjectType { get; set; }
        public Status? Status { get; set; }
        public string CustomerId { get; set; }
        public string ExpertId { get; set; }
        public string PlatformId { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? ReportDateFrom { get; set; }
        public DateTime? ReportDateTo { get; set; }
        public int? MinKeywordCount { get; set; }
        public int? MaxKeywordCount { get; set; }
        public decimal? MinCompletionRate { get; set; }
    }

    public class ProjectMetricsDto
    {
        public int TotalKeywords { get; set; }
        public int CompletedKeywords { get; set; }
        public decimal CompletionRate { get; set; }
        public int TotalPromotions { get; set; }
        public int ActiveSmPlatforms { get; set; }
        public int TotalTodos { get; set; }
        public decimal TotalBudget { get; set; }
    }

    public class TimelineEventDto
    {
        public string Date { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
    }

    public class KeywordPerformanceDto
    {
        public string KeywordName { get; set; }
        public bool IsCompleted { get; set; }
        public string CompletionDate { get; set; }
    }

    public class BudgetAnalysisDto
    {
        public decimal TotalBudget { get; set; }
        public decimal UsedBudget { get; set; }
        public decimal RemainingBudget { get; set; }
        public List<BudgetItemDto> BudgetItems { get; set; }
    }

    public class BudgetItemDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }

    public class DashboardProjectsDto
    {
        public List<ProjectSummaryDto> ThisWeekProjects { get; set; }
        public List<ProjectSummaryDto> PastProjects { get; set; }
        public List<ProjectSummaryDto> DueThisWeekView { get; set; }
        public List<ProjectSummaryDto> PastView { get; set; }
    }
}