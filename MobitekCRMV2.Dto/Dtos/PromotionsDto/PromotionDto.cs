using MobitekCRMV2.Dto.Dtos.CustomersDto;
using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;
using MobitekCRMV2.Dto.Dtos.ProjectsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.PromotionsDto
{
    public class PromotionDto
    {
        public string Id { get; set; }
        public string PromotionURL { get; set; }
        public string LandingPage { get; set; }
        public string Date { get; set; }
        public Status Status { get; set; }
        public Status GoogleIndex { get; set; }

        public List<NewsSiteSummaryDto> NewsSites { get; set; }
        public List<KeywordSummaryDto> Keywords { get; set; }
        public string ProjectId { get; set; }
        public ProjectSummaryDto Project { get; set; }
        public string UserId { get; set; }
        public UserSummaryDto User { get; set; }
        public string CustomerId { get; set; }
        public CustomerSummaryDto Customer { get; set; }
        public int NewsSiteCount { get; set; }
        public int KeywordCount { get; set; }
    }
    public class PromotionListDto
    {
        public List<PromotionSummaryDto> Promotions { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<Status, int> StatusDistribution { get; set; }
    }
    public class PromotionSummaryDto
    {
        public string Id { get; set; }
        public string PromotionURL { get; set; }
        public string LandingPage { get; set; }
        public string Date { get; set; }
        public Status Status { get; set; }
        public Status GoogleIndex { get; set; }
        public int NewsSiteCount { get; set; }
    }

    public class PromotionCreateUpdateDto
    {
        public string PromotionURL { get; set; }
        public string LandingPage { get; set; }
        public DateTime Date { get; set; }
        public Status Status { get; set; }
        public Status GoogleIndex { get; set; }
        public string ProjectId { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
    }
    public class PromotionFilterDto
    {
        public string SearchTerm { get; set; }
        public Status? Status { get; set; }
        public Status? GoogleIndex { get; set; }
        public string ProjectId { get; set; }
        public string CustomerId { get; set; }
        public string UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? HasNewsSites { get; set; }
    }
 

    public class TimelineDto
    {
        public string CreatedDate { get; set; }
        public string LastUpdateDate { get; set; }
        public List<StatusChangeDto> StatusChanges { get; set; }
    }

    public class StatusChangeDto
    {
        public string Date { get; set; }
        public Status OldStatus { get; set; }
        public Status NewStatus { get; set; }
    }
}