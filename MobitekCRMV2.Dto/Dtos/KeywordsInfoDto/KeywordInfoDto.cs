using MobitekCRMV2.Dto.Dtos.KeywordsDto;

namespace MobitekCRMV2.Dto.Dtos.KeywordsInfoDto
{
    public class KeywordInfoDto
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public int Page { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeywordId { get; set; }
        public KeywordSummaryDto Keyword { get; set; }
    }

    public class KeywordInfoListDto
    {
        public List<KeywordInfoSummaryDto> KeywordInfos { get; set; }
        public int TotalCount { get; set; }
    }
    public class KeywordInfoSummaryDto
    {
        public string Id { get; set; }
        public int Position { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
    }

    public class KeywordInfoCreateUpdateDto
    {
        public int Position { get; set; }
        public int Page { get; set; }
        public string Domain { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string KeywordId { get; set; }
    }
    public class KeywordInfoFilterDto
    {
        public string Domain { get; set; }
        public string KeywordId { get; set; }
        public int? MinPosition { get; set; }
        public int? MaxPosition { get; set; }
        public int? Page { get; set; }
    }
}