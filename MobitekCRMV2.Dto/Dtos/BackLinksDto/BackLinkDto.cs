using MobitekCRMV2.Dto.Dtos.DomainDto;
using MobitekCRMV2.Dto.Dtos.NewsSitesDto;

namespace MobitekCRMV2.Dto.Dtos.BackLinkDto
{
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
        public string LastUpdateDate { get; set; }  
        public string ManualCheckDate { get; set; }

        public string DomainId { get; set; }
        public DomainSummaryDto Domain { get; set; }
        public string NewsSiteId { get; set; }
        public NewsSiteSummaryDto NewsSite { get; set; }
    }

    public class BackLinkListDto
    {
        public List<BackLinkSummaryDto> BackLinks { get; set; }
        public int TotalCount { get; set; }
    }

    public class BackLinkSummaryDto
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string UrlFrom { get; set; }
        public string LandingPage { get; set; }
        public string Anchor { get; set; }
        public int Da { get; set; }
        public int Pa { get; set; }
        public string LastUpdateDate { get; set; }
    }

    public class BackLinkCreateUpdateDto
    {
        public string Status { get; set; }
        public string UrlFrom { get; set; }
        public string LandingPage { get; set; }
        public string Anchor { get; set; }
        public string SelectDate { get; set; }
        public int Da { get; set; }
        public int Pa { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime ManualCheckDate { get; set; }
        public string DomainId { get; set; }
        public string NewsSiteId { get; set; }
    }
    public class BackLinkFilterDto
    {
        public string Status { get; set; }
        public string DomainId { get; set; }
        public string NewsSiteId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinDa { get; set; }
        public int? MaxDa { get; set; }
        public int? MinPa { get; set; }
        public int? MaxPa { get; set; }
    }
}