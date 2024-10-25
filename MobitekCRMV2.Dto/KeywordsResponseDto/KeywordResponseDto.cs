using MobitekCRMV2.Dto.Dtos.KeywordsDto;

namespace MobitekCRMV2.Dto.Dtos.KeywordsResponseDto
{
    public class KeywordResponseDto
    {
        public string Id { get; set; }
        public string SerpURL { get; set; }
        public string Date { get; set; }
        public string Position { get; set; }
        public string KeywordId { get; set; }
        public KeywordSummaryDto Keyword { get; set; }
    }
    public class KeywordResponseListDto
    {
        public List<KeywordResponseSummaryDto> KeywordResponses { get; set; }
        public int TotalCount { get; set; }
    }

    public class KeywordResponseSummaryDto
    {
        public string Id { get; set; }
        public string Position { get; set; }
        public string Date { get; set; }
    }
    public class KeywordResponseCreateUpdateDto
    {
        public string SerpURL { get; set; }
        public DateTime Date { get; set; }
        public string Position { get; set; }
        public string KeywordId { get; set; }
    }

    public class KeywordResponseFilterDto
    {
        public string KeywordId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Position { get; set; }
    }
    public class KeywordResponseBulkCreateDto
    {
        public string KeywordId { get; set; }
        public List<KeywordResponseCreateUpdateDto> Responses { get; set; }
    }

    public class KeywordResponseAnalysisDto
    {
        public string KeywordId { get; set; }
        public string KeywordName { get; set; }
        public string FirstPosition { get; set; }
        public string CurrentPosition { get; set; }
        public string BestPosition { get; set; }
        public string PositionChange { get; set; } 
        public List<PositionHistoryDto> History { get; set; }
    }

    public class PositionHistoryDto
    {
        public string Date { get; set; }
        public string Position { get; set; }
    }
}