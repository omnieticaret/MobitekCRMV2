using MobitekCRMV2.Dto.Dtos.BackLinkDto;
using MobitekCRMV2.Dto.Dtos.PromotionsDto;
using MobitekCRMV2.Dto.Dtos.UsersDto;

namespace MobitekCRMV2.Dto.Dtos.NewsSitesDto
{
    public class NewsSiteDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string GoogleNews { get; set; }
        public int DRScore { get; set; }
        public int LinkedDomains { get; set; }
        public int TotalBacklinks { get; set; }
        public int OrganicTraffic { get; set; }
        public int AllTraffic { get; set; }
        public int PAScore { get; set; }
        public int DAScore { get; set; }
        public int SpamScore { get; set; }
        public string EditorMail { get; set; }
        public string EditorPhone { get; set; }
        public int MozDA { get; set; }
        public string LastUpdateDate { get; set; }
        public string OldData { get; set; }
        public string Note { get; set; }

        public string PromotionId { get; set; }
        public PromotionSummaryDto Promotion { get; set; }
        public string UserId { get; set; }
        public UserSummaryDto User { get; set; }
        public List<BackLinkSummaryDto> BackLinks { get; set; }

        public int BackLinksCount { get; set; }
        public string TrafficQuality { get; set; }
    }

    public class NewsSiteListDto
    {
        public List<NewsSiteSummaryDto> NewsSites { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, double> AverageMetrics { get; set; }
    }

    public class NewsSiteSummaryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int DAScore { get; set; }
        public int PAScore { get; set; }
        public string EditorMail { get; set; }
        public string LastUpdateDate { get; set; }
        public int BackLinksCount { get; set; }
    }
    public class NewsSiteCreateUpdateDto
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public string GoogleNews { get; set; }
        public int DRScore { get; set; }
        public int LinkedDomains { get; set; }
        public int TotalBacklinks { get; set; }
        public int OrganicTraffic { get; set; }
        public int AllTraffic { get; set; }
        public int PAScore { get; set; }
        public int DAScore { get; set; }
        public int SpamScore { get; set; }
        public string EditorMail { get; set; }
        public string EditorPhone { get; set; }
        public int MozDA { get; set; }
        public string Note { get; set; }
        public string PromotionId { get; set; }
        public string UserId { get; set; }
    }

    public class NewsSiteFilterDto
    {
        public string Name { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? MinDAScore { get; set; }
        public int? MaxDAScore { get; set; }
        public int? MinPAScore { get; set; }
        public int? MaxPAScore { get; set; }
        public string UserId { get; set; }
        public string PromotionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


}