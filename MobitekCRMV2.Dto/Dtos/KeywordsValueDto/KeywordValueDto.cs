using MobitekCRMV2.Dto.Dtos.KeywordsDto;
using MobitekCRMV2.Entity.Enums;

namespace MobitekCRMV2.Dto.Dtos.KeywordsValueDto
{
    public class KeywordValueDto
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public int Page { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public TargetCheckType TargetCheckType { get; set; }
        public string KeywordId { get; set; }
        public KeywordSummaryDto Keyword { get; set; }
    }

    public class KeywordValueListDto
    {
        public List<KeywordValueSummaryDto> KeywordValues { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, int> PositionDistribution { get; set; }
    }

    public class KeywordValueSummaryDto
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public string CountryCode { get; set; }
        public string CreatedDate { get; set; }
        public TargetCheckType TargetCheckType { get; set; }
    }
    public class KeywordValueCreateUpdateDto
    {
        public int Position { get; set; }
        public int Page { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
        public TargetCheckType TargetCheckType { get; set; }
        public string KeywordId { get; set; }
    }

    public class KeywordValueFilterDto
    {
        public string KeywordId { get; set; }
        public string CountryCode { get; set; }
        public TargetCheckType? TargetCheckType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MinPosition { get; set; }
        public int? MaxPosition { get; set; }
        public string Domain { get; set; }
    }

}